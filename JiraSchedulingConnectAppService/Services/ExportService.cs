﻿using JiraSchedulingConnectAppService.Common;
using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.DBModels;
using ModelLibrary.DTOs.Algorithm.ScheduleResult;
using ModelLibrary.DTOs.Export;
using Newtonsoft.Json;
using System.Dynamic;
using UtilsLibrary.Exceptions;
using Task = System.Threading.Tasks.Task;

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


            // Create startdate field
            //dynamic request = new ExpandoObject();
            //request.description = "Task Start Time";
            //request.name = "Start Date";
            //request.searchKey = "com.atlassian.jira.plugin.system.customfieldtypes:datetimerange";
            //request.type = ""

            //var respone = await jiraAPI.Post("rest/api/3/field", userCreate);
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
    }
}
