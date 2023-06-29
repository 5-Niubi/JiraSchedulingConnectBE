using JiraSchedulingConnectAppService.Common;
using ModelLibrary.DBModels;

namespace JiraSchedulingConnectAppService.Services
{
    public class JiraProjectService
    {
        private readonly APICommon api;

        public JiraProjectService(JiraDemoContext db, IConfiguration config)
        {
            api = new APICommon(db, config);
        }
        public async Task<string> GetAllProject(HttpContext context)
        {
            var response = await api.Get("/rest/api/3/project/search", context);
            string responseContent = string.Empty;
            if (response.IsSuccessStatusCode)
            {
                responseContent = await response.Content.ReadAsStringAsync();
            }else
            {
                throw new Exception("Error");
            }
            return responseContent;
        }
    }
}
