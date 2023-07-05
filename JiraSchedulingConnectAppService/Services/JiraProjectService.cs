using JiraSchedulingConnectAppService.Common;
using JiraSchedulingConnectAppService.Services.Interfaces;
using ModelLibrary.DBModels;

namespace JiraSchedulingConnectAppService.Services
{
    public class JiraProjectService
    {
        private readonly IJiraBridgeAPIService jiraApi;

        public JiraProjectService(IJiraBridgeAPIService jiraApi)
        {
            this.jiraApi = jiraApi;
        }
        public async Task<string> GetAllProject(HttpContext context)
        {
            var response = await jiraApi.Get("/rest/api/3/project/search");
            string responseContent = string.Empty;
            if (response.IsSuccessStatusCode)
            {
                responseContent = await response.Content.ReadAsStringAsync();
            }
            else
            {
                throw new Exception("Error");
            }
            return responseContent;
        }
    }
}
