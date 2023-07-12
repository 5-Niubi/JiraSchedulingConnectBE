using Aspose.Tasks.Saving;
using Aspose.Tasks;
using JiraSchedulingConnectAppService.Common;
using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.DBModels;
using ModelLibrary.DTOs.Algorithm.ScheduleResult;
using ModelLibrary.DTOs.Export;
using Newtonsoft.Json;
using System.Dynamic;
using UtilsLibrary.Exceptions;
using Task = System.Threading.Tasks.Task;
using Humanizer.Localisation;

namespace JiraSchedulingConnectAppService.Services
{
    public class ExportService : IExportService
    {
        private readonly JiraDemoContext db;
        private readonly IJiraBridgeAPIService jiraAPI;
        public ExportService(JiraDemoContext db, IJiraBridgeAPIService jiraAPI)
        {
            this.db = db;
            this.jiraAPI = jiraAPI;
        }

        async public Task<string> ToJira(int scheduleId, string projectJiraId)
        {
            var schedule = await db.Schedules.Where(s => s.Id == scheduleId)
                .FirstOrDefaultAsync();
            if (schedule == null)
            {
                throw new NotFoundException(Const.MESSAGE.NOTFOUND_SCHEDULE);
            }
            var tasks = JsonConvert.DeserializeObject<List<TaskScheduleResultDTO>>(schedule.Tasks);

            // Create jira user and get user accountId into workerDict
            var workerCreatedDict = await JiraCreateWorkForce(tasks);

            var bulkTasks = await JiraCreateBulkTask(tasks, workerCreatedDict, projectJiraId);
            return bulkTasks;
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



        private async Task<Dictionary<int?, WorkforceScheduleResultDTO>> JiraCreateWorkForce(List<TaskScheduleResultDTO> tasks)
        {
            var workderDict = new Dictionary<int?, WorkforceScheduleResultDTO>();
            tasks.ForEach(t => workderDict.Add(t.workforce.id, t.workforce));

            foreach (var worker in workderDict.Values)
            {
                var userCreate = new CreateJiraUserDTO.Request();
                userCreate.displayName = worker.displayName;
                userCreate.emailAddress = worker.email;

                var respone = await jiraAPI.Post("rest/api/3/user", userCreate);
                var responseObj = await respone.Content.ReadFromJsonAsync<CreateJiraUserDTO.Response>();
                worker.accountId = responseObj.accountId;
            }
            return workderDict;
        }

        private async Task JiraCreateIssueCustomField()
        {

            // Start date : customfield_10015
            // Planed start: customfield_10061
            // Planed end: customfield_10053
        }

        private async Task<string> JiraCreateBulkTask(List<TaskScheduleResultDTO> tasks,
            Dictionary<int?, WorkforceScheduleResultDTO> workderDict, string projectJiraId)
        {
            dynamic request = new ExpandoObject();
            request.issueUpdates = new List<ExpandoObject>();
            dynamic issueUpdate;

            foreach (var task in tasks)
            {
                // each issueUpdate is each task
                issueUpdate = new ExpandoObject();
                dynamic fields = new ExpandoObject();

                fields.assignee.id = workderDict[task.workforce.id].accountId;

                fields.customfield_10061 = task.startDate; // Planed Start
                fields.customfield_10053 = task.endDate; // Planed End
                fields.đueate = task.endDate;

                fields.project.id = projectJiraId;

                fields.project.reporter.id = "";
                fields.summary = task.name;

                issueUpdate.fields = fields;
                request.issueUpdates.Add(issueUpdate);
            }

            var respone = await jiraAPI.Post("rest/api/3/issue/bulk", request);
            return await respone.Content.ReadAsStringAsync();
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
            project.Save(memoryStream, SaveFileFormat.Mpp);

            // Reset the MemoryStream position to the beginning
            memoryStream.Position = 0;

            // Use the MemoryStream as needed, such as sending it through an API response
            // For example, you could return it in a controller action as a FileStreamResult:
            // return File(memoryStream, "application/octet-stream", "project.mpp");
            return memoryStream;
        }

    }
}
