using AutoMapper;
using java.lang;
using java.time;
using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ModelLibrary.DBModels;
using ModelLibrary.DTOs.Algorithm.ScheduleResult;
using ModelLibrary.DTOs.Export;
using ModelLibrary.DTOs.Projects;
using ModelLibrary.DTOs.Thread;
using net.sf.mpxj;
using net.sf.mpxj.MpxjUtilities;
using net.sf.mpxj.mspdi;
using net.sf.mpxj.writer;
using Newtonsoft.Json;
using System.Dynamic;
using System.Text.RegularExpressions;
using UtilsLibrary;
using UtilsLibrary.Exceptions;
using Duration = net.sf.mpxj.Duration;
using Task = System.Threading.Tasks.Task;
using TimeUnit = net.sf.mpxj.TimeUnit;

namespace JiraSchedulingConnectAppService.Services
{
    public class ExportService : IExportService
    {
        private readonly WoTaasContext db;
        private readonly IJiraBridgeAPIService jiraAPI;
        private readonly HttpContext http;
        private readonly string appName;
        private readonly IThreadService threadService;
        private readonly IMapper mapper;
        private IConfiguration config;

        public ExportService(WoTaasContext db, IJiraBridgeAPIService jiraAPI,
            IHttpContextAccessor httpAccessor, IConfiguration config,
            IThreadService threadService, IMapper mapper
            )
        {
            this.db = db;
            this.jiraAPI = jiraAPI;
            http = httpAccessor.HttpContext;

            this.config = config;
            appName = config.GetValue<string>("Environment:Appname");
            this.threadService = threadService;
            this.mapper = mapper;
        }

        public async Task<ThreadStartDTO> ToJira(int scheduleId, string projectKey, string? projectName)
        {
            var schedule = await db.Schedules.Where(s => s.Id == scheduleId)
               .Include(s => s.Parameter).ThenInclude(p => p.Project)
               .FirstOrDefaultAsync() ??
                throw new NotFoundException(Const.MESSAGE.NOTFOUND_SCHEDULE);

            var cloudId = new JWTManagerService(http).GetCurrentCloudId();
            var accountId = db.AtlassianTokens.Where(tk => tk.CloudId == cloudId)
                                    .First().AccountInstalledId;

            string threadId = ThreadService.CreateThreadId();
            threadId = threadService.StartThread(threadId,
                async () => await ProcessToJiraThread(
                    threadId, schedule, accountId, projectKey, projectName
                    ));
            return new ThreadStartDTO(threadId);
        }

        async public Task<(string, MemoryStream)> ToMSProject(int scheduleId, string token)
        {
            // Validate token
            var jwtManage = new JWTManagerService(config);
            if (!jwtManage.ValidateJwt(token))
            {
                throw new UnAuthorizedException();
            };

            var schedule = (await db.Schedules.Where(s => s.Id == scheduleId)
               .Include(s => s.Parameter).ThenInclude(p => p.Project)
               .FirstOrDefaultAsync()) ??
               throw new NotFoundException(Const.MESSAGE.NOTFOUND_SCHEDULE);

            var tasks = JsonConvert.DeserializeObject<List<TaskScheduleResultDTO>>(schedule.Tasks);
            (var workforceResultDict, var workforceEmailDict) = ExtractWorkforceFromResultSchedule(tasks);

            return XMLCreateFile(tasks, schedule.Parameter.Project, workforceResultDict);
        }

        private async Task ProcessToJiraThread(string threadId, Schedule schedule,
            string? accountId, string projectKey, string? projectName)
        {
            try
            {
                var thread = threadService.GetThreadModel(threadId);
                try
                {
                    var tasks = JsonConvert.DeserializeObject<List<TaskScheduleResultDTO>>(schedule.Tasks);
                    thread.Progress = "Prepare Jira screen fields";
                    var prepareResult = await JiraPrepareForSync(schedule.Parameter.Project, accountId, thread, projectName, projectKey);

                    (var workforceResultDict, var workforceEmailDict) = ExtractWorkforceFromResultSchedule(tasks);

                    thread.Progress = "Create worker selection";
                    var workerCreatedDict = await JiraCreateWorkForce(prepareResult.WorkerFieldContext,
                        prepareResult.FieldDict["Worker"], workforceResultDict);

                    thread.Progress = "Finding assignee";
                    var workerEmailDict = await JiraGetExistUserIdByEmail(workforceEmailDict);

                    thread.Progress = "Importing tasks";
                    var bulkTasks = await JiraCreateBulkTask(tasks, workerCreatedDict, prepareResult);

                    thread.Progress = "Linking tasks";
                    await JiraCreateIssueLink(tasks, bulkTasks);

                    // Update the thread status and result when finished
                    thread.Status = Const.THREAD_STATUS.SUCCESS;
                    dynamic result = new ExpandoObject();
                    result.projectKey = prepareResult.ProjectKey;
                    result.projectName = prepareResult.ProjectName;
                    thread.Result = result;
                }
                catch (JiraAPIException ex)
                {
                    thread.Status = Const.THREAD_STATUS.ERROR;
                    dynamic error = new ExpandoObject();
                    error.message = ex.Message;
                    error.response = ex.jiraResponse;
                    thread.Result = error;
                }
                catch (System.Exception ex)
                {
                    thread.Status = Const.THREAD_STATUS.ERROR;

                    dynamic error = new ExpandoObject();
                    error.message = ex.Message;
                    error.stackTrace = ex.StackTrace;

                    thread.Result = error;
                }
            }
            catch
            {
                /* Do nothing*/
            }

        }

        private (Dictionary<int, WorkforceScheduleResultDTO>,
            Dictionary<string, WorkforceScheduleResultDTO>)
            ExtractWorkforceFromResultSchedule(List<TaskScheduleResultDTO> tasks)
        {
            // Todo mapping workforce from result, not from parameter


            var worforceDiction = new Dictionary<int, WorkforceScheduleResultDTO>();

            tasks.ForEach(t =>
            {
                if (!worforceDiction.ContainsKey(t.workforce.id))
                    worforceDiction.Add(t.workforce.id, t.workforce);
            });

            var workforceEmailDiction = new Dictionary<string, WorkforceScheduleResultDTO>();
            foreach (var wf in worforceDiction.Values)
            {
                if (!workforceEmailDiction.ContainsKey(wf.email))
                    workforceEmailDiction.Add(wf.email, wf);
            }

            // Return wkeremail and wkerdict
            return (worforceDiction, workforceEmailDiction);
        }

        private async Task JiraCreateIssueLink(List<TaskScheduleResultDTO> tasks, Dictionary<int?, string> issueIdDict)
        {
            HttpResponseMessage respone;
            foreach (var task in tasks)
            {
                if (task.taskIdPrecedences.Count == 0)
                {
                    continue;
                }
                foreach (var taskIdPre in task.taskIdPrecedences)
                {
                    dynamic body = new ExpandoObject();
                    body.inwardIssue = new ExpandoObject();
                    body.inwardIssue.id = issueIdDict[taskIdPre];
                    body.outwardIssue = new ExpandoObject();
                    body.outwardIssue.id = issueIdDict[task.id];
                    body.type = new ExpandoObject();
                    body.type.name = "Blocks";

                    respone = await jiraAPI.Post($"rest/api/3/issueLink", body);
                }
            }
        }

        private async Task<Dictionary<string, WorkforceScheduleResultDTO>> JiraGetExistUserIdByEmail
            (Dictionary<string, WorkforceScheduleResultDTO> workerEmailDict)
        {
            HttpResponseMessage respone;
            respone = await jiraAPI.Get($"rest/api/3/users/search");
            var rawUserInfos = await respone.Content.ReadAsStringAsync();
            var userinfos = JsonConvert.DeserializeObject<List<JiraAPIGetUsersResDTO.Root>>(rawUserInfos);
            foreach (var userinfo in userinfos)
            {
                string? emailAddr = userinfo.emailAddress;
                if (emailAddr != null && workerEmailDict.ContainsKey(emailAddr)
                     && !workerEmailDict[emailAddr].accountId.IsNullOrEmpty())
                {
                    workerEmailDict[emailAddr].accountId = userinfo.accountId;
                }
            }

            return workerEmailDict;
        }

        private async Task<Dictionary<int, WorkforceScheduleResultDTO>> JiraCreateWorkForce(
            string fieldContextId,
            string? fieldId, Dictionary<int, WorkforceScheduleResultDTO> workerDict)
        {
            HttpResponseMessage respone;

            int startAt = 0;
            var url = $"rest/api/3/field/{fieldId}/context/{fieldContextId}/option?startAt={startAt}";
            var workerOptionValueList = new List<JiraAPICreateFieldOptionResDTO.Option>();
            var isLast = false;
            // Check is worker exist, if not create a new worker
            do
            {
                respone = await jiraAPI.Get(url);
                var pagingWorkerJson = await respone.Content.ReadFromJsonAsync<JiraAPIResponsePagingDTO<JiraAPICreateFieldOptionResDTO.Option>>();
                isLast = pagingWorkerJson.IsLast;
                workerOptionValueList.AddRange(pagingWorkerJson.Values);

                startAt += pagingWorkerJson.MaxResults;
                url = $"rest/api/3/field/{fieldId}/context/{fieldContextId}/option?startAt={startAt}";
            } while (!isLast);

            dynamic body = new ExpandoObject();
            body.options = new List<ExpandoObject>();

            var workerCreateList = new List<WorkforceScheduleResultDTO>();

            foreach (var worker in workerDict.Values)
            {
                // Must convert to English characters
                var workerName = $"{worker.displayName} - {worker.email}";

                // Check is worker exist
                var workerFound = workerOptionValueList.Where(e => e.Value == workerName).FirstOrDefault();
                if (workerFound != null)
                {
                    worker.fieldOptiontId = workerFound.Id;
                    continue;
                }

                workerCreateList.Add(worker);
                dynamic option = new ExpandoObject();
                option.disabled = false;
                option.value = workerName;

                body.options.Add(option);
            }

            if (body.options.Count > 0)
            {
                respone = await jiraAPI.Post($"rest/api/3/field/{fieldId}/context/{fieldContextId}/option", body);
                var responseObj = await respone.Content.ReadFromJsonAsync<JiraAPICreateFieldOptionResDTO.Root>();

                for (int i = 0; i < workerCreateList.Count; i++)
                {
                    workerCreateList[i].fieldOptiontId = responseObj.Options[i].Id;
                }
            }

            return workerDict;
        }

        private async Task<JiraAPIPrepareResultDTO> JiraPrepareForSync(
            ModelLibrary.DBModels.Project project, string accountId, ThreadModel thread,
            string? projectNameCreate, string projectKeyCreate)
        {
            /* TODO: - Tối ưu việc config field
                     - Tối ưu việc quản lý các scheme
             */
            thread.Progress = "Creating Project";
            var projectId = await JiraCreateProject(accountId, projectKeyCreate, projectNameCreate);

            thread.Progress = "Verifying Project";
            (var projectKey, var projectName) = await JiraVerifyProject(projectId);

            var issueTypeId = await JiraCreateIssueType(projectKey);

            thread.Progress = "Creating custom field";
            var fieldDict = await JiraCreateCustomField(projectKey);

            thread.Progress = "Creating custom field context";
            var workerFieldContext = await JiraCreateFieldContext(fieldDict["Worker"],
                projectKey, projectId, issueTypeId);

            thread.Progress = "Creating custom screen";
            var screenId = await JiraCreateScreen(projectKey);

            thread.Progress = "Creating screen tab";
            var screenTabId = await JiraCreateScreenTab(screenId);

            thread.Progress = "Add field into screen";
            await JiraAddFieldIntoScreen(screenId, screenTabId, fieldDict);

            thread.Progress = "Create screen scheme";
            var screenSchemeId = await JiraCreateScreenScheme(screenId, projectKey);

            thread.Progress = "Create issue type screen scheme";
            var issueTypeScreenSchemeId = await JiraCreateIssueTypeScreenScheme(issueTypeId, screenSchemeId, projectKey);

            thread.Progress = "Create issue type scheme";
            var issueTypeSchemeId = await JiraCreateIssueTypeScheme(issueTypeId, projectKey);


            await JiraAssignIssueTypeScreenSchemeWithProject(issueTypeScreenSchemeId, projectId);
            await JiraAssignIssueTypeSchemeWithProject(issueTypeSchemeId, projectId);

            var result = new JiraAPIPrepareResultDTO();
            result.FieldDict = fieldDict;
            result.ProjectId = projectId;
            result.ProjectKey = projectKey;
            result.ProjectName = projectName;
            result.IssueTypeId = issueTypeId;
            result.WorkerFieldContext = workerFieldContext;
            return result;
        }

        private async Task JiraAssignIssueTypeSchemeWithProject(string? issueTypeSchemeId, int projectId)
        {
            dynamic body = new ExpandoObject();
            body.issueTypeSchemeId = issueTypeSchemeId;
            body.projectId = projectId;

            HttpResponseMessage respone = await jiraAPI.Put($"rest/api/3/issuetypescheme/project", body);
        }

        private async Task JiraAssignIssueTypeScreenSchemeWithProject(string? issueTypeScreenSchemeId, int projectId)
        {
            dynamic body = new ExpandoObject();
            body.issueTypeScreenSchemeId = issueTypeScreenSchemeId;
            body.projectId = projectId;

            HttpResponseMessage respone = await jiraAPI.Put($"rest/api/3/issuetypescreenscheme/project", body);
        }

        private async Task<string> JiraCreateIssueTypeScheme(string? issueTypeId, string projectKey)
        {
            var issueTypeSchemeName = $"{appName} Issue Type Scheme for {projectKey}";
            // Check if exist then get id
            HttpResponseMessage respone = await jiraAPI.Get($"rest/api/3/issuetypescheme?queryString={issueTypeSchemeName}");
            var result = await respone.Content.ReadFromJsonAsync<JiraAPIResponsePagingDTO<JiraAPISearchIssueTypeSchemeResDTO.Root>>();

            if (result.Values.Count > 0)
            {
                return result.Values[0].Id;
            }

            dynamic body = new ExpandoObject();
            body.description = issueTypeSchemeName;
            body.name = issueTypeSchemeName;
            body.issueTypeIds = new List<string>();
            body.issueTypeIds.Add(issueTypeId);

            respone = await jiraAPI.Post($"rest/api/3/issuetypescheme", body);

            var id = (await respone.Content.ReadFromJsonAsync<JiraAPICreateIssueTypeSchemeResDTO>()).IssueTypeSchemeId;
            return id;

        }

        private async Task<string> JiraCreateIssueTypeScreenScheme(string? issueTypeId, int? screenSchemeId, string projectKey)
        {
            var issueTypeScreenSchemeName = $"{appName} Issue Type Screen Scheme for {projectKey}";
            // Check if exist then get id
            HttpResponseMessage respone = await jiraAPI.Get($"rest/api/3/issuetypescreenscheme?queryString={issueTypeScreenSchemeName}");
            var result = await respone.Content.ReadFromJsonAsync<JiraAPIResponsePagingDTO<JiraAPISearchIssueTypeScreenSchemeDTO>>();

            if (result.Values.Count > 0)
            {
                return result.Values[0].Id;
            }

            dynamic body = new ExpandoObject();
            body.name = issueTypeScreenSchemeName;
            body.issueTypeMappings = new List<ExpandoObject>();

            dynamic issueTypeMapping = new ExpandoObject();
            issueTypeMapping.issueTypeId = "default";
            issueTypeMapping.screenSchemeId = $"{screenSchemeId}";
            body.issueTypeMappings.Add(issueTypeMapping);
            issueTypeMapping = new ExpandoObject();

            issueTypeMapping.issueTypeId = issueTypeId;
            issueTypeMapping.screenSchemeId = $"{screenSchemeId}";
            body.issueTypeMappings.Add(issueTypeMapping);

            respone = await jiraAPI.Post($"rest/api/3/issuetypescreenscheme", body);

            var id = (await respone.Content.ReadFromJsonAsync<JiraCreateIssueTypeScreenSchemeResDTO>()).Id;
            return id;

        }

        private async Task<int?> JiraCreateScreenScheme(int screenId, string projectKey)
        {
            var screenSchemeName = $"{appName} screen scheme for {projectKey}";
            // Check if exist then get id
            HttpResponseMessage respone = await jiraAPI.Get($"rest/api/3/screenscheme?queryString={screenSchemeName}");
            var result = await respone.Content.ReadFromJsonAsync<JiraAPIResponsePagingDTO<JiraAPISearchScreenSchemeResDTO.Root>>();

            if (result.Values.Count > 0)
            {
                return result.Values[0].Id;
            }

            dynamic body = new ExpandoObject();
            body.description = string.Concat(screenSchemeName);
            body.name = string.Concat(screenSchemeName);
            body.screens = new ExpandoObject();
            body.screens.@default = screenId;
            body.screens.create = screenId;
            body.screens.edit = screenId;
            body.screens.view = screenId;

            respone = await jiraAPI.Post($"rest/api/3/screenscheme", body);
            var id = (await respone.Content.ReadFromJsonAsync<JiraAPICreateScreenSchemeResDTO>()).Id;
            return id;
        }

        private async Task JiraAddFieldIntoScreen(int screenId, int? tabId, Dictionary<string, string?> fieldDict)
        {

            foreach (var value in fieldDict.Values)
            {
                dynamic body = new ExpandoObject();
                body.fieldId = value;
                try
                {
                    await jiraAPI.Post($"rest/api/3/screens/{screenId}/tabs/{tabId}/fields", body);
                }
                catch (JiraAPIException) { }
            }

        }

        private async Task<int?> JiraCreateScreenTab(int screenId)
        {
            var tabname = "Field Tab";
            HttpResponseMessage respone = await jiraAPI.Get($"rest/api/3/screens/{screenId}/tabs");
            var result = await respone.Content.ReadFromJsonAsync<List<JiraAPIScreenTabResDTO>>();
            var fieldTab = result.Where(r => r.Name == tabname).First();
            return fieldTab.Id;
        }

        private async Task<Dictionary<string, string?>> JiraCreateCustomField(string projectKey)
        {
            var fieldDict = new Dictionary<string, string?>();
            // Kiểm tra tồn tại, nếu tồn tại thì lấy luôn
            HttpResponseMessage respone = await jiraAPI.Get("rest/api/3/field");
            var result = await respone.Content.ReadFromJsonAsync<List<JiraAPIFieldResultDTO>>();

            // Summary: Input
            var issType = result.Where(r => r.Name == "Summary").First();
            fieldDict.Add(issType.Name, issType.Id);

            // Description: Text
            issType = result.Where(r => r.Name == "Description").First();
            fieldDict.Add(issType.Name, issType.Id);

            // Labels: Multiselect
            issType = result.Where(r => r.Name == "Labels").First();
            fieldDict.Add(issType.Name, issType.Id);

            // Target start: Date
            issType = result.Where(r => r.Name == "Target start").First();
            fieldDict.Add(issType.Name, issType.Id);

            // Target end: Date
            issType = result.Where(r => r.Name == "Target end").First();
            fieldDict.Add(issType.Name, issType.Id);

            // Linked Issue: Linked
            issType = result.Where(r => r.Name == "Linked Issues").First();
            fieldDict.Add(issType.Name, issType.Id);

            // Assignee: Assignee
            issType = result.Where(r => r.Name == "Assignee").First();
            fieldDict.Add(issType.Name, issType.Id);

            // Worker: Select
            var workerFieldName = $"Worker";
            issType = result.Where(r => r.Name == workerFieldName).FirstOrDefault();
            if (issType != null)
            {
                fieldDict.Add("Worker", issType.Id);
            }
            else
            {
                dynamic body = new ExpandoObject();
                body.name = workerFieldName;
                body.description = $"Worker Assign By {appName}";
                body.type = "com.atlassian.jira.plugin.system.customfieldtypes:select";

                respone = await jiraAPI.Post("rest/api/3/field", body);
                var field = (await respone.Content.ReadFromJsonAsync<JiraAPICreateIssueFieldResDTO.Root>());
                fieldDict.Add("Worker", field.Id);
            }
            return fieldDict;
        }

        private async Task<string> JiraCreateFieldContext(string workerFieldId, string projectKey, int projectId, string issueTypeId)
        {
            HttpResponseMessage response;

            response = await jiraAPI.Get($"rest/api/3/field/{workerFieldId}/context/projectmapping");
            var contextProjetMapping = (await response.Content.ReadFromJsonAsync
                <JiraAPIResponsePagingDTO<JiraAPIGetCustomFieldContextProjectMappingResDTO.Root>>());
            var contextProject = contextProjetMapping.Values.FirstOrDefault(e => e.projectId == projectId.ToString());
            if (contextProject != null)
            {
                return contextProject.contextId;
            }

            var contextName = $"{appName} field context for {projectKey} worker field";

            dynamic body = new ExpandoObject();
            body.name = contextName;
            body.projectIds = new string[] { projectId.ToString() };
            body.issueTypeIds = new string[] { issueTypeId.ToString() };

            try
            {
                response = await jiraAPI.Post($"rest/api/3/field/{workerFieldId}/context", body);
                var id = (await response.Content.ReadFromJsonAsync<JiraAPICreateFieldContextResDTO.Root>()).Id;
                return id;
            }
            catch (JiraAPIException ex)
            {
                var responseErr = new JiraAPIErrorResDTO.Root();
                string contextId = "";
                if (ex.jiraResponse != null)
                {
                    responseErr = JsonConvert.DeserializeObject<JiraAPIErrorResDTO.Root>(ex.jiraResponse);
                    Regex pattern = new(@"These projects are already associated with a context: (?<contextId>[\w]+).");
                    Match match = pattern.Match(responseErr.errorMessages[0]);
                    contextId = match.Groups["contextId"].Value;
                }
                return contextId;
            }

        }

        private async Task<string> JiraCreateIssueType(string projectKey)
        {
            var issueTypeName = $"Task From {appName}";
            // Kiểm tra tồn tại, nếu tồn tại thì lấy luôn
            HttpResponseMessage respone = await jiraAPI.Get("rest/api/3/issuetype");
            var result = await respone.Content.ReadFromJsonAsync<List<JiraAPIIssueTypeResDTO>>();
            var issType = result.Where(r => r.Name == issueTypeName).FirstOrDefault();
            if (issType != null)
            {
                return issType.Id;
            }
            // If not exist, than create a new one
            dynamic body = new ExpandoObject();
            body.name = issueTypeName;
            body.description = "com.pyxis.greenhopper.jira:gh-simplified-basic";
            body.type = "standard";

            respone = await jiraAPI.Post("rest/api/3/issuetype", body);
            var id = (await respone.Content.ReadFromJsonAsync<JiraAPICreateIssueTypeResDTO>()).Id;
            return id;
        }

        private async Task<int> JiraCreateProject(string accountId, string projectKey, string? projectName)
        {
            HttpResponseMessage respone;
            // Kiểm tra tồn tại, nếu tồn tại thì lấy luôn
            try
            {
                respone = await jiraAPI.Get($"rest/api/3/project/{projectKey}");
                var result = await respone.Content.ReadFromJsonAsync<JiraAPIGetProjectResDTO.Root>()
                    ?? throw new JiraAPIException();

                return Convert.ToInt32(result.id);
            }
            catch (JiraAPIException)
            {
                // Ignore exception if it return not found
            }

            // If not exist, than create a new one
            dynamic body = new ExpandoObject();
            body.leadAccountId = accountId;
            body.name = projectName;
            body.key = projectKey;
            body.projectTemplateKey = "com.pyxis.greenhopper.jira:gh-simplified-basic";
            body.projectTypeKey = "software";

            respone = await jiraAPI.Post("rest/api/3/project", body);
            var id = (await respone.Content.ReadFromJsonAsync<JiraAPICreatProjectResponseDTO>())?.Id;
            return id ?? 0;
        }

        private async Task<(string, string)> JiraVerifyProject(int projectId)
        {
            HttpResponseMessage respone;
            respone = await jiraAPI.Get($"rest/api/3/project/{projectId}");
            var result = await respone.Content.ReadFromJsonAsync<JiraAPIGetProjectResDTO.Root>();
            return (result.key, result.name);
        }

        private async Task<int> JiraCreateScreen(string projectKey)
        {
            var screenName = $"{appName} screen {projectKey}";
            // Check if screen exist then get id
            HttpResponseMessage respone = await jiraAPI.Get($"rest/api/3/screens?queryString={screenName}");
            var result = (await respone.Content.ReadFromJsonAsync<JiraAPIResponsePagingDTO<JiraAPIScreenResDTO>>());
            if (result.Values.Count > 0)
            {
                return result.Values[0].Id;
            }
            // If not exist, than create a new one
            dynamic body = new ExpandoObject();
            body.name = screenName;
            body.description = "com.pyxis.greenhopper.jira:gh-simplified-basic";

            respone = await jiraAPI.Post("rest/api/3/screens", body);
            var id = (await respone.Content.ReadFromJsonAsync<JiraAPIScreenResDTO>()).Id;
            return id;
        }



        private async Task<Dictionary<int?, string>> JiraCreateBulkTask(List<TaskScheduleResultDTO> tasks,
            Dictionary<int, WorkforceScheduleResultDTO> workderDict, JiraAPIPrepareResultDTO prepare)
        {
            dynamic request = new ExpandoObject();
            request.issueUpdates = new List<ExpandoObject>();
            dynamic issueUpdate;

            foreach (var task in tasks)
            {
                // each issueUpdate is each task
                issueUpdate = new ExpandoObject();
                dynamic fields = new ExpandoObject();
                if (workderDict.ContainsKey(task.workforce.id) && workderDict[task.workforce.id].accountId != null)
                {
                    fields.assignee = new ExpandoObject();
                    fields.assignee.id = workderDict[task.workforce.id].accountId;
                }
                Utils.AddPropertyToExpando(fields, prepare.FieldDict["Target start"], task.startDate.Value.ToString("yyyy-MM-dd"));
                Utils.AddPropertyToExpando(fields, prepare.FieldDict["Target end"], task.endDate.Value.ToString("yyyy-MM-dd"));

                dynamic workerField = new ExpandoObject();
                workerField.id = workderDict[task.workforce.id].fieldOptiontId;
                Utils.AddPropertyToExpando(fields, prepare.FieldDict["Worker"], workerField);
                fields.project = new ExpandoObject();
                fields.project.id = prepare.ProjectId;

                fields.project.reporter = new ExpandoObject();
                fields.project.reporter.id = "";
                fields.summary = task.name;
                fields.issuetype = new ExpandoObject();
                fields.issuetype.id = prepare.IssueTypeId; //Type task

                issueUpdate.fields = fields;
                request.issueUpdates.Add(issueUpdate);
            }

            HttpResponseMessage respone = await jiraAPI.Post("rest/api/3/issue/bulk", request);

            var issueCreatedResult = await respone.Content.ReadFromJsonAsync<JiraAPICreateBulkTaskResDTO.Root>();
            var issueIdDict = new Dictionary<int?, string>();

            for (int i = 0; i < tasks.Count; i++)
            {
                issueIdDict.Add(tasks[i].id, issueCreatedResult.Issues[i].Id);
            }

            return issueIdDict;
        }

        private (string, MemoryStream) XMLCreateFile(List<TaskScheduleResultDTO> tasks, ModelLibrary.DBModels.Project projectDb,
            Dictionary<int, WorkforceScheduleResultDTO> workforceResultDict)
        {
            var wokingTimesString = JsonConvert.DeserializeObject<List<WorkingTimeDTO>>(projectDb.WorkingTimes);

            var resourceDict = new Dictionary<int?, net.sf.mpxj.Resource>();
            var taskDict = new Dictionary<int?, net.sf.mpxj.Task>();
            var milestoneDict = new Dictionary<int, net.sf.mpxj.Task>();


            ProjectFile project = new();
            var projectFileName = $"{projectDb.Name}.xml";

            var calendar = project.AddDefaultBaseCalendar();

            calendar.setWorkingDay(java.time.DayOfWeek.SATURDAY, true);
            calendar.setWorkingDay(java.time.DayOfWeek.SUNDAY, true);
            java.time.DayOfWeek[] weeks = {java.time.DayOfWeek.MONDAY, java.time.DayOfWeek.TUESDAY,
                java.time.DayOfWeek.WEDNESDAY, java.time.DayOfWeek.WEDNESDAY, java.time.DayOfWeek.THURSDAY
                , java.time.DayOfWeek.FRIDAY, java.time.DayOfWeek.SATURDAY, java.time.DayOfWeek.SUNDAY};
            foreach (var day in weeks)
            {
                var hours = calendar.GetCalendarHours(day);
                hours.clear();
                if (wokingTimesString.IsNullOrEmpty())
                {
                    var startTime = LocalTime.of(8, 0);
                    var finishTime = LocalTime.of(12, 0);
                    hours.add(new LocalTimeRange(startTime, finishTime));

                    startTime = LocalTime.of(13, 0);
                    finishTime = LocalTime.of(17, 0);
                    hours.add(new LocalTimeRange(startTime, finishTime));
                    continue;
                }
                foreach (var timeRange in wokingTimesString)
                {
                    var start = TimeOnly.Parse(timeRange.Start);
                    var startTime = LocalTime.of(start.Hour, start.Minute);
                    var finish = TimeOnly.Parse(timeRange.Finish);
                    var finishTime = LocalTime.of(finish.Hour, finish.Minute);
                    hours.add(new LocalTimeRange(startTime, finishTime));
                }
            }

            foreach (var key in workforceResultDict.Keys)
            {
                if (!resourceDict.ContainsKey(key))
                {
                    var rs = project.AddResource();
                    rs.Name = workforceResultDict[key].displayName;
                    rs.Cost = new Float((float)workforceResultDict[key].unitSalary);
                    rs.OverAllocated = true;
                    resourceDict.Add(workforceResultDict[key].id, rs);
                }
            }

            foreach (var t in tasks)
            {
                net.sf.mpxj.Task? milestone = null;

                if (t.mileStone != null && !milestoneDict.ContainsKey(t.mileStone.id))
                {
                    milestone = project.AddTask();
                    milestone.Name = t.mileStone.name;
                    milestoneDict.Add(t.mileStone.id, milestone);
                }
                else if (t.mileStone != null && milestoneDict.ContainsKey(t.mileStone.id))
                {
                    milestone = milestoneDict[t.mileStone.id];
                }

                net.sf.mpxj.Task? task = null;
                if (milestone != null)
                {
                    task = milestone.addTask();
                }
                else
                {
                    task = project.AddTask();
                }
                task.TaskMode = TaskMode.MANUALLY_SCHEDULED;
                task.Name = t.name;


                var start = t.startDate.Value;
                task.Start = LocalDateTime.of(start.Year, start.Month, start.Day, 8, 0);

                task.Duration = Duration.getInstance((double)t.duration * (double)projectDb.BaseWorkingHour,
                    TimeUnit.HOURS);

                var end = t.endDate.Value;
                task.Finish = LocalDateTime.of(end.Year, end.Month, end.Day, 17, 0);

                var assignment = task.AddResourceAssignment(resourceDict[t.workforce.id]);

                taskDict.Add(t.id, task);
            }

            foreach (var t in tasks)
            {
                if (t.taskIdPrecedences.Count == 0)
                    continue;
                foreach (var pre in t.taskIdPrecedences)
                {
                    taskDict[t.id].AddPredecessor(taskDict[pre], RelationType.FINISH_START, null);
                }
            }

            ProjectWriter writer = ProjectWriterUtility.getProjectWriter(projectFileName);

            MemoryStream memStream = new();
            DotNetOutputStream stream = new(memStream);
            writer.write(project, stream);
            memStream.Position = 0;
            return (projectFileName, memStream);
        }

        public async Task<string> JiraRequest(dynamic dynamic)
        {
            string url = "rest/api/3/field";
            string method = "GET";
            dynamic body = new ExpandoObject();

            body.issueTypeSchemeId = "10176";
            body.projectId = "10007";
            HttpResponseMessage respone;
            switch (method)
            {
                case "GET":
                    respone = await jiraAPI.Get(url);
                    break;

                case "POST":
                    respone = await jiraAPI.Post(url, body);
                    break;
                case "PUT":
                    respone = await jiraAPI.Put(url, body);
                    break;
                case "DELETE":
                    respone = await jiraAPI.Delete(url);
                    break;
                default:
                    return "";
            }
            return await respone.Content.ReadAsStringAsync();
        }

    }
}
