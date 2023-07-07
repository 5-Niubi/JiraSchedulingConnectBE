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
            body.assigneeType = "UNASSIGNED";
            body.avatarId = 10200;
            body.categoryId = 10120;
            body.description = "Cloud migration initiative";
            body.issueSecurityScheme = 10001;
            body.key = "EXPWEC";
            //body.LeadAccountId = "5b10a0effa615349cb016cd8";
            body.name = "Example Project With Export Controller";
            body.notificationScheme = 10021;
            body.permissionScheme = 10011;
            body.projectTemplateKey = "com.pyxis.greenhopper.jira:gh-simplified-basic";
            body.projectTypeKey = "software";
            await jiraAPI.Post("rest/api/3/project", body);
        }
    }
}
