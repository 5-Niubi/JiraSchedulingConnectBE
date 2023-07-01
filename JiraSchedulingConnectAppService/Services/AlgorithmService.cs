using AlgorithmServiceServer.DTOs.AlgorithmController;
using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.DBModels;
using System.Text.Json;

namespace JiraSchedulingConnectAppService.Services
{
    public class AlgorithmService : IAlgorithmService
    {
        private readonly IConfiguration config;
        private readonly HttpClient client;
        private readonly JiraDemoContext db;
        private readonly HttpContext http;
        public AlgorithmService(IConfiguration configuration, JiraDemoContext db, IHttpContextAccessor httpContextAccessor)
        {
            config = configuration;
            this.client = new HttpClient();
            this.db = db;
            http = httpContextAccessor.HttpContext;
        }

        public void TestConverter(int projectId)
        {
            var baseUrl = config.GetValue<string[]>("Environment:AlgorithmServiceDomains");
            var jwt = new JWTManagerService(http);

            var contentObject = new InputToORDTO();
            var projectFromDB = db.Projects .Where(p => p.CloudId == jwt.GetCurrentCloudId())
                .Include(p => p.Tasks).FirstOrDefault();
            var workerFromDB = db.Workforces.Where(w => w.CloudId == jwt.GetCurrentCloudId()).ToList();

            contentObject.StartDate = (DateTime) projectFromDB.StartDate;
            contentObject.Budget = (int) projectFromDB.Budget;
            contentObject.Deadline = (int) (projectFromDB.Deadline.Value - projectFromDB.StartDate.Value).TotalDays;
            contentObject.TaskList =  projectFromDB.Tasks.ToList();
            contentObject.WorkerList = workerFromDB;


            var content = new StringContent(JsonSerializer.Serialize(contentObject));
            client.PostAsync(baseUrl[0] + "api/Algorithm/TestConverter", content);
        }

    }
}
