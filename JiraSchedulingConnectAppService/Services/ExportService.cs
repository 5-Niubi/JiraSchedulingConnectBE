using AutoMapper;
using java.lang;
using JiraSchedulingConnectAppService.Common;
using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.DBModels;
using ModelLibrary.DTOs.Algorithm.ScheduleResult;
using ModelLibrary.DTOs.Export;
using ModelLibrary.DTOs.Thread;
using net.sf.mpxj;
using net.sf.mpxj.MpxjUtilities;
using net.sf.mpxj.writer;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading;
using UtilsLibrary.Exceptions;
using Duration = net.sf.mpxj.Duration;
using Task = System.Threading.Tasks.Task;

namespace JiraSchedulingConnectAppService.Services
{
    public class ExportService : IExportService
    {
        private readonly JiraDemoContext db;
        private readonly IJiraBridgeAPIService jiraAPI;
        private readonly HttpContext http;
        private readonly string appName;
        private readonly IThreadService threadService;
        private readonly IMapper mapper;
        private IConfiguration config;

        public ExportService(JiraDemoContext db, IJiraBridgeAPIService jiraAPI,
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

        public async Task<ThreadStartDTO> ToJira(int scheduleId)
        {
            var schedule = await db.Schedules.Where(s => s.Id == scheduleId)
               .Include(s => s.Parameter).ThenInclude(p => p.Project)
               .FirstOrDefaultAsync() ??
                throw new NotFoundException(Const.MESSAGE.NOTFOUND_SCHEDULE);

            var parameterWorkers = await db.ParameterResources.Where(pr => pr.ParameterId == schedule.ParameterId
                && pr.Type == Const.RESOURCE_TYPE.WORKFORCE).Include(pr => pr.ResourceNavigation)
                .ToListAsync();

            var worforceDiction = new Dictionary<int, Workforce>();
            parameterWorkers.ForEach(pr =>
            {
                if (!worforceDiction.ContainsKey(pr.ResourceId))
                    worforceDiction.Add(pr.ResourceId, pr.ResourceNavigation);
            });
            var workforceResultDict = mapper.Map<Dictionary<int, WorkforceScheduleResultDTO>>(worforceDiction);

            var workforceEmailDiction = new Dictionary<string, WorkforceScheduleResultDTO>();
            foreach (var wf in workforceResultDict.Values)
            {
                if (!workforceEmailDiction.ContainsKey(wf.email))
                    workforceEmailDiction.Add(wf.email, wf);
            }

            var cloudId = new JWTManagerService(http).GetCurrentCloudId();
            var accountId = db.AtlassianTokens.Where(tk => tk.CloudId == cloudId)
                                    .First().AccountInstalledId;

            string threadId = ThreadService.CreateThreadId();
            threadId = threadService.StartThread(threadId,
                async () => await ProcessToJiraThread(
                    threadId, schedule, accountId, workforceResultDict, workforceEmailDiction
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

            var parameterWorkers = await db.ParameterResources.Where(pr => pr.ParameterId == schedule.ParameterId
                && pr.Type == Const.RESOURCE_TYPE.WORKFORCE).Include(pr => pr.ResourceNavigation)
                .ToListAsync();
            var worforceDiction = new Dictionary<int, Workforce>();
            parameterWorkers.ForEach(pr =>
            {
                if (!worforceDiction.ContainsKey(pr.ResourceId))
                    worforceDiction.Add(pr.ResourceId, pr.ResourceNavigation);
            });
            var workforceResultDict = mapper.Map<Dictionary<int, WorkforceScheduleResultDTO>>(worforceDiction);

            var tasks = JsonConvert.DeserializeObject<List<TaskScheduleResultDTO>>(schedule.Tasks);

            return XMLCreateFile(tasks, schedule.Parameter.Project, workforceResultDict);
        }

        private async Task ProcessToJiraThread(string threadId, Schedule schedule, string? accountId,
            Dictionary<int, WorkforceScheduleResultDTO> workforceResultDict,
            Dictionary<string, WorkforceScheduleResultDTO> workforceEmailDict)
        {
            try
            {
                var thread = threadService.GetThreadModel(threadId);
                try
                {
                    var tasks = JsonConvert.DeserializeObject<List<TaskScheduleResultDTO>>(schedule.Tasks);
                    thread.Progress = "Prepare Jira screen fields";
                    var prepareResult = await JiraPrepareForSync(schedule.Parameter.Project, accountId, thread);

                    thread.Progress = "Create worker selection";
                    var workerCreatedDict = await JiraCreateWorkForce(tasks, prepareResult.FieldDict["Worker"], workforceResultDict);

                    thread.Progress = "Finding assignee";
                    var workerEmailDict = await JiraGetExistUserIdByEmail(workforceEmailDict);

                    thread.Progress = "Importing tasks";
                    var bulkTasks = await JiraCreateBulkTask(tasks, workerCreatedDict, prepareResult);

                    thread.Progress = "Linking tasks";
                    await JiraCreateIssueLink(tasks, bulkTasks);

                    // Update the thread status and result when finished
                    thread.Status = Const.THREAD_STATUS.SUCCESS;
                    dynamic result = new ExpandoObject();
                    result.projectId = prepareResult.ProjectId;
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

        private async Task<Dictionary<string, WorkforceScheduleResultDTO>> JiraGetExistUserIdByEmail(Dictionary<string, WorkforceScheduleResultDTO> workerEmailDict)
        {
            HttpResponseMessage respone;
            respone = await jiraAPI.Get($"rest/api/3/users/search");
            var rawUserInfos = await respone.Content.ReadAsStringAsync();
            var userinfos = JsonConvert.DeserializeObject<List<JiraAPIGetUsersResDTO.Root>>(rawUserInfos);
            foreach (var userinfo in userinfos)
            {
                string? emailAddr = userinfo.emailAddress;
                if (emailAddr != null && workerEmailDict.ContainsKey(emailAddr))
                {
                    workerEmailDict[emailAddr].accountId = userinfo.accountId;
                }
            }

            return workerEmailDict;
        }

        private async Task<Dictionary<int, WorkforceScheduleResultDTO>> JiraCreateWorkForce(List<TaskScheduleResultDTO> tasks, string? fieldId, Dictionary<int, WorkforceScheduleResultDTO> workerDict)
        {
            //var workderDict = new Dictionary<int, WorkforceScheduleResultDTO>();
            //tasks.ForEach(t => { workderDict.TryAdd(t.workforce.id, t.workforce); });

            HttpResponseMessage respone;
            respone = await jiraAPI.Get($"rest/api/3/field/{fieldId}/context");
            var pagingContextJson = await respone.Content.ReadFromJsonAsync<JiraAPIResponsePagingDTO<JiraAPIGetIssueCustomFieldContextResDTO>>();
            var contextFound = pagingContextJson.Values.Where(e => e.Name == "Default Configuration Scheme for Worker").FirstOrDefault();
            if (contextFound == null)
            {
                throw new NotFoundException("Not Found Context Of Worker Field");
            }

            // Check is worker exist, if not create a new worker
            respone = await jiraAPI.Get($"rest/api/3/field/{fieldId}/context/{contextFound.Id}/option");
            var pagingWorkerJson = await respone.Content.ReadFromJsonAsync<JiraAPIResponsePagingDTO<JiraAPICreateFieldOptionResDTO.Option>>();

            dynamic body = new ExpandoObject();
            body.options = new List<ExpandoObject>();

            var workerCreateList = new List<WorkforceScheduleResultDTO>();

            foreach (var worker in workerDict.Values)
            {
                // Must convert to English characters
                var workerName = Utils.ConvertVN($"{worker.displayName} - {worker.email}");

                // Check is worker exist
                var workerFound = pagingWorkerJson.Values.Where(e => e.Value == workerName).FirstOrDefault();
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
                respone = await jiraAPI.Post($"rest/api/3/field/{fieldId}/context/{contextFound.Id}/option", body);
                var responseObj = await respone.Content.ReadFromJsonAsync<JiraAPICreateFieldOptionResDTO.Root>();

                for (int i = 0; i < workerCreateList.Count; i++)
                {
                    workerCreateList[i].fieldOptiontId = responseObj.Options[i].Id;
                }
            }

            return workerDict;
        }

        private async Task<JiraAPIPrepareResultDTO> JiraPrepareForSync(
            ModelLibrary.DBModels.Project project, string accountId, ThreadModel thread)
        {
            /* TODO: - Tối ưu việc config field
                     - Tối ưu việc quản lý các scheme
             */

            thread.Progress = "Creating custom field";
            var fieldDict = await JiraCreateCustomField();

            thread.Progress = "Creating custom screen";
            var screenId = await JiraCreateScreen();

            var screenTabId = await JiraCreateScreenTab(screenId);
            await JiraAddFieldIntoScreen(screenId, screenTabId, fieldDict);
            var screenSchemeId = await JiraCreateScreenScheme(screenId);

            var issueTypeId = await JiraCreateIssueType();

            var issueTypeScreenSchemeId = await JiraCreateIssueTypeScreenScheme(issueTypeId, screenSchemeId);
            var issueTypeSchemeId = await JiraCreateIssueTypeScheme(issueTypeId);

            thread.Progress = "Creating Project";
            (var projectId, var projectName) = await JiraCreateProject(project, accountId);

            await JiraAssignIssueTypeScreenSchemeWithProject(issueTypeScreenSchemeId, projectId);
            await JiraAssignIssueTypeSchemeWithProject(issueTypeSchemeId, projectId);

            var result = new JiraAPIPrepareResultDTO();
            result.FieldDict = fieldDict;
            result.ProjectId = projectId;
            result.ProjectName = projectName;
            result.IssueTypeId = issueTypeId;
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

        private async Task<string> JiraCreateIssueTypeScheme(string? issueTypeId)
        {
            var issueTypeSchemeName = $"{appName} Issue Type Scheme";
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

        private async Task<string> JiraCreateIssueTypeScreenScheme(string? issueTypeId, int? screenSchemeId)
        {
            var issueTypeScreenSchemeName = $"{appName} Issue Type Screen Scheme";
            // Check if exist then get id
            HttpResponseMessage respone = await jiraAPI.Get($"rest/api/3/issuetypescreenscheme?queryString={issueTypeScreenSchemeName}");
            var result = await respone.Content.ReadFromJsonAsync<JiraAPIResponsePagingDTO<JiraAPISearchIssueTypeScreenSchemeDTO>>();

            if (result.Values.Count > 0)
            {
                return result.Values[0].Id;
            }

            dynamic body = new ExpandoObject();
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

        private async Task<int?> JiraCreateScreenScheme(int screenId)
        {
            var screenSchemeName = $"{appName} screen scheme";
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
                catch (JiraAPIException ex) { }
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

        private async Task<Dictionary<string, string?>> JiraCreateCustomField()
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
            issType = result.Where(r => r.Name == "Worker").FirstOrDefault();
            if (issType != null)
            {
                fieldDict.Add(issType.Name, issType.Id);
            }
            else
            {
                dynamic body = new ExpandoObject();
                body.name = "Worker";
                body.description = $"Worker Assign By {appName}";
                body.type = "com.atlassian.jira.plugin.system.customfieldtypes:select";

                respone = await jiraAPI.Post("rest/api/3/field", body);
                var field = (await respone.Content.ReadFromJsonAsync<JiraAPICreateIssueFieldResDTO.Root>());
                fieldDict.Add(field.Name, field.Id);
            }
            return fieldDict;
        }

        private async Task<string> JiraCreateIssueType()
        {
            var issueTypeName = string.Concat("Task From", Const.SPACE, appName);
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

        private async Task<(int, string)> JiraCreateProject(ModelLibrary.DBModels.Project project, string accountId)
        {
            // TODO: Xử lý trùng key       

            HttpResponseMessage respone;
            int count = 0;
            var projectName = $"{project.Name}";
            bool isContinue = true;
            do
            {
                respone = await jiraAPI.Get($"rest/api/3/project/search?query={projectName}");
                var result = await respone.Content.ReadFromJsonAsync<JiraAPIResponsePagingDTO<JiraAPISearchProjectResDTO.Root>>();
                if (result.Total > 0)
                {
                    projectName = $"{project.Name}-{result.Total + 1}";
                }
                else
                {
                    isContinue = false;
                }
            }
            while (isContinue);
            dynamic body = new ExpandoObject();
            var key = Utils.ExtractUpperDigitLetter(projectName);
            body.key = key;
            body.leadAccountId = accountId;
            body.name = projectName;
            body.projectTemplateKey = "com.pyxis.greenhopper.jira:gh-simplified-basic";
            body.projectTypeKey = "software";

            respone = await jiraAPI.Post("rest/api/3/project", body);
            var id = (await respone.Content.ReadFromJsonAsync<JiraAPICreatProjectResponseDTO>()).Id;
            return (id, projectName);
        }

        private async Task<int> JiraCreateScreen()
        {
            var screenName = string.Concat(appName, Const.SPACE, "screen");
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
            ProjectFile project = new ProjectFile();
            var projectFileName = $"{projectDb.Name}.xml";
            var resourceDict = new Dictionary<int?, net.sf.mpxj.Resource>();
            var taskDict = new Dictionary<int?, net.sf.mpxj.Task>();
            var milestoneDict = new Dictionary<int, net.sf.mpxj.Task>();


            foreach (var key in workforceResultDict.Keys)
            {
                if (!resourceDict.ContainsKey(key))
                {
                    var rs = project.AddResource();
                    rs.Name = workforceResultDict[key].displayName;
                    rs.Cost = new Float((float)workforceResultDict[key].unitSalary);

                    resourceDict.Add(workforceResultDict[key].id, rs);
                }
            }


            //tasks.ForEach(t =>
            //{
            //    if (!resourceDict.ContainsKey(t.workforce.id))
            //    {
            //        var rs = project.AddResource();
            //        rs.Name = t.workforce.displayName;
            //        rs.Cost = new Float((float)t.workforce.unitSalary);

            //        resourceDict.Add(t.workforce.id, rs);
            //    }
            //});
            foreach (var t in tasks)
            {
                net.sf.mpxj.Task milestone;

                if (!milestoneDict.ContainsKey(t.mileStone.id))
                {
                    milestone = project.AddTask();
                    milestone.Name = t.mileStone.name;
                    milestoneDict.Add(t.mileStone.id, milestone);
                }
                else
                {
                    milestone = milestoneDict[t.mileStone.id];
                }

                var task = milestone.addTask();

                task.Start = t.startDate.Value.ToJavaLocalDateTime();
                task.Finish = t.endDate.Value.ToJavaLocalDateTime();
                task.Duration = Duration.getInstance((double)t.duration, TimeUnit.DAYS);
                task.Name = t.name;

                var assignment = task.AddResourceAssignment(resourceDict[t.workforce.id]);

                taskDict.Add(t.id, task);
            }

            foreach (var t in tasks)
            {
                if (t.taskIdPrecedences.Count == 0) continue;
                foreach (var pre in t.taskIdPrecedences)
                {
                    taskDict[t.id].AddPredecessor(taskDict[pre], RelationType.FINISH_START, null);
                }
            }

            ProjectWriter writer = ProjectWriterUtility.getProjectWriter(projectFileName);

            MemoryStream memStream = new MemoryStream();
            DotNetOutputStream stream = new DotNetOutputStream(memStream);
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
                    respone = await jiraAPI.Put(url, body); break;
                case "DELETE":
                    respone = await jiraAPI.Delete(url); break;
                default: return "";
            }
            return await respone.Content.ReadAsStringAsync();
        }

    }
}
