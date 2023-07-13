using Aspose.Tasks;
using Aspose.Tasks.Saving;
using com.sun.xml.@internal.fastinfoset.util;
using JiraSchedulingConnectAppService.Common;
using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using ModelLibrary.DBModels;
using ModelLibrary.DTOs.Algorithm.ScheduleResult;
using ModelLibrary.DTOs.Export;
using Newtonsoft.Json;
using System.Collections.Immutable;
using System.Dynamic;
using System.Linq;
using System.Text;
using UtilsLibrary.Exceptions;
using Task = System.Threading.Tasks.Task;

namespace JiraSchedulingConnectAppService.Services
{
    public class ExportService : IExportService
    {
        private readonly JiraDemoContext db;
        private readonly IJiraBridgeAPIService jiraAPI;
        private readonly HttpContext http;
        private readonly string appName;
        public ExportService(JiraDemoContext db, IJiraBridgeAPIService jiraAPI,
            IHttpContextAccessor httpAccessor, IConfiguration config)
        {
            this.db = db;
            this.jiraAPI = jiraAPI;
            http = httpAccessor.HttpContext;

            appName = config.GetValue<string>("Environment:Appname");
        }

        async public Task<string> ToJira(int scheduleId)
        {
            var schedule = await db.Schedules.Where(s => s.Id == scheduleId)
                .Include(s => s.Parameter).ThenInclude(p => p.Project)
                .FirstOrDefaultAsync();

            if (schedule == null)
            {
                throw new NotFoundException(Const.MESSAGE.NOTFOUND_SCHEDULE);
            }
            var tasks = JsonConvert.DeserializeObject<List<TaskScheduleResultDTO>>(schedule.Tasks);

            //// Create jira user and get user accountId into workerDict


            var prepareResult = await JiraPrepareForSync(schedule.Parameter.Project);
            var workerCreatedDict = await JiraCreateWorkForce(tasks, prepareResult.FieldDict["Worker"]);
            var bulkTasks = await JiraCreateBulkTask(tasks, workerCreatedDict, prepareResult);
            await JiraCreateIssueLink(tasks, bulkTasks);

            return "";
        }

        async public Task<MemoryStream> ToMSProject(int scheduleId)
        {
            var schedule = await db.Schedules.Where(s => s.Id == scheduleId)
                .FirstOrDefaultAsync();
            if (schedule == null)
            {
                throw new NotFoundException(Const.MESSAGE.NOTFOUND_SCHEDULE);
            }
            var tasks = JsonConvert.DeserializeObject<List<TaskScheduleResultDTO>>(schedule.Tasks);
            return MppCreateFile(tasks);
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

        private async Task<Dictionary<int?, WorkforceScheduleResultDTO>> JiraCreateWorkForce(List<TaskScheduleResultDTO> tasks, string? fieldId)
        {
            var workderDict = new Dictionary<int?, WorkforceScheduleResultDTO>();
            tasks.ForEach(t => { workderDict.TryAdd(t.workforce.id, t.workforce); });

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

            foreach (var worker in workderDict.Values)
            {
                var workerName = $"{worker.displayName} - {worker.email}";

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

            if(body.options.Count > 0)
            {
                respone = await jiraAPI.Post($"rest/api/3/field/{fieldId}/context/{contextFound.Id}/option", body);
                var responseObj = await respone.Content.ReadFromJsonAsync<JiraAPICreateFieldOptionResDTO.Root>();

                for (int i = 0; i < workerCreateList.Count; i++)
                {
                    workerCreateList[i].fieldOptiontId = responseObj.Options[i].Id;
                }
            }

            return workderDict;
        }

        private async Task<JiraAPIPrepareResultDTO> JiraPrepareForSync(
            ModelLibrary.DBModels.Project project)
        {
            /* TODO: - Tối ưu việc config field
                     - Tối ưu việc quản lý các scheme
             */
            var fieldDict = await JiraCreateCustomField();
            var screenId = await JiraCreateScreen();

            var screenTabId = await JiraCreateScreenTab(screenId);
            await JiraAddFieldIntoScreen(screenId, screenTabId, fieldDict);
            var screenSchemeId = await JiraCreateScreenScheme(screenId);

            var issueTypeId = await JiraCreateIssueType();

            var issueTypeScreenSchemeId = await JiraCreateIssueTypeScreenScheme(issueTypeId, screenSchemeId);
            var issueTypeSchemeId = await JiraCreateIssueTypeScheme(issueTypeId);

            var projectId = await JiraCreateProject(project);

            await JiraAssignIssueTypeScreenSchemeWithProject(issueTypeScreenSchemeId, projectId);
            await JiraAssignIssueTypeSchemeWithProject(issueTypeSchemeId, projectId);

            var result = new JiraAPIPrepareResultDTO();
            result.FieldDict = fieldDict;
            result.ProjectId = projectId;
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
            try
            {

                foreach (var value in fieldDict.Values)
                {
                    dynamic body = new ExpandoObject();
                    body.fieldId = value;
                    await jiraAPI.Post($"rest/api/3/screens/{screenId}/tabs/{tabId}/fields", body);
                }
            }
            catch (JiraAPIException ex) { }

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

        private async Task<int> JiraCreateProject(ModelLibrary.DBModels.Project project)
        {
            // TODO: Xử lý trùng key
            var cloudId = new JWTManagerService(http).GetCurrentCloudId();
            var accountId = db.AtlassianTokens.Where(tk => tk.CloudId == cloudId)
                .First().AccountInstalledId;
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
                    projectName = $"{project.Name}-{++count}";
                }
                else
                {
                    isContinue = false;
                }
            }
            while (isContinue);
            dynamic body = new ExpandoObject();
            var key = string.Concat(Utils.ExtractUpperLetter(project.Name), project.Id, count);
            body.key = key;
            body.leadAccountId = accountId;
            body.name = projectName;
            body.projectTemplateKey = "com.pyxis.greenhopper.jira:gh-simplified-basic";
            body.projectTypeKey = "software";

            respone = await jiraAPI.Post("rest/api/3/project", body);
            var id = (await respone.Content.ReadFromJsonAsync<JiraAPICreatProjectResponseDTO>()).Id;
            return id;
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
            Dictionary<int?, WorkforceScheduleResultDTO> workderDict, JiraAPIPrepareResultDTO prepare)
        {
            dynamic request = new ExpandoObject();
            request.issueUpdates = new List<ExpandoObject>();
            dynamic issueUpdate;

            foreach (var task in tasks)
            {
                // each issueUpdate is each task
                issueUpdate = new ExpandoObject();
                dynamic fields = new ExpandoObject();

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

            for (int i = 0; i< tasks.Count; i++)
            {
                issueIdDict.Add(tasks[i].id, issueCreatedResult.Issues[i].Id);
            }
                
            return issueIdDict;
        }

        private MemoryStream MppCreateFile(List<TaskScheduleResultDTO> tasks)
        {
            var taskDict = new Dictionary<int?, Aspose.Tasks.Task>();

            var project = new Aspose.Tasks.Project();

            foreach (var t in tasks)
            {
                // Add tasks
                var task = project.RootTask.Children.Add(t.name);
                task.Set(Tsk.PercentComplete, 0);
                task.Set(Tsk.Start, (DateTime)t.startDate);
                task.Set(Tsk.Finish, (DateTime)t.endDate);

                // Resource
                var resource = project.Resources.Add(t.workforce.name);
                project.ResourceAssignments.Add(task, resource);

                taskDict.Add(t.id, task);
            }

            // Set task predecessor
            foreach (var t in tasks)
            {
                if (t.taskIdPrecedences.Count != 0)
                {
                    foreach (var taskPred in t.taskIdPrecedences)
                    {
                        var link = project.TaskLinks.Add(taskDict[taskPred], taskDict[t.id]);
                        link.LinkType = TaskLinkType.FinishToStart;
                    }
                }

            }
            // Save the project as MPP
            var memoryStream = new MemoryStream();

            // Save the project as MPP to the MemoryStream
            project.Save("project.mpp", SaveFileFormat.Mpp);

            // Reset the MemoryStream position to the beginning
            memoryStream.Position = 0;

            // Use the MemoryStream as needed, such as sending it through an API response
            // For example, you could return it in a controller action as a FileStreamResult:
            // return File(memoryStream, "application/octet-stream", "project.mpp");
            return memoryStream;
        }

        public async Task<string> JiraRequest(dynamic dynamic)
        {
            string url = "rest/api/3/issuetypescreenscheme?queryString=Default Issue Type ";
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
