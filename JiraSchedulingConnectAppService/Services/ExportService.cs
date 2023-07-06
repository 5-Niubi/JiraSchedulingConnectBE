using JiraSchedulingConnectAppService.Services.Interfaces;
using ModelLibrary.DBModels;
using ModelLibrary.DTOs.Export;
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

        async public Task ToJira()
        {
            var body = new CreateJiraProjectDTO();
            //body.AssigneeType = "PROJECT_LEAD";
            //body.AvatarId = 10200;
            //body.CategoryId = 10120;
            //body.Description = "Cloud migration initiative";
            //body.IssueSecurityScheme = 10001;
            body.key = "EX";
            //body.LeadAccountId = "5b10a0effa615349cb016cd8";
            body.name = "Example";
            //body.NotificationScheme = 10021;
            //body.PermissionScheme = 10011;
            body.projectTemplateKey = "com.pyxis.greenhopper.jira:gh-simplified-basic";
            body.projectTypeKey = "software";
            await jiraAPI.Post("rest/api/3/project", body);
        }
    }
}
