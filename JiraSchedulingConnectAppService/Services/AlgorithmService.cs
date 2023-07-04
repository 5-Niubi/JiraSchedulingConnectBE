using AlgorithmServiceServer;
using AlgorithmServiceServer.DTOs.AlgorithmController;
using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.DBModels;
using ModelLibrary.DTOs.AlgorithmController;
using System.Text.Json;

namespace JiraSchedulingConnectAppService.Services
{
    public class AlgorithmService : IAlgorithmService
    {
        private readonly HttpClient client;
        private readonly JiraDemoContext db;
        private readonly HttpContext http;
        private readonly IAPIMicroserviceService apiMicro;
        public AlgorithmService( JiraDemoContext db, IHttpContextAccessor httpContextAccessor, IAPIMicroserviceService apiMicro)
        {
            this.client = new HttpClient();
            this.db = db;
            http = httpContextAccessor.HttpContext;
            this.apiMicro = apiMicro;
        }

        public async Task<OutputFromORDTO> TestConverter(int projectId)
        {
            //var jwt = new JWTManagerService(http);
            //var contentObject = new InputToORDTO();

            //var projectFromDB = db.Projects .Where(p => p.CloudId == jwt.GetCurrentCloudId())
            //    .Include(p => p.Tasks).FirstOrDefault();
            //var workerFromDB = db.Workforces.Where(w => w.CloudId == jwt.GetCurrentCloudId()).ToList();
            //var skillFromDB = db.Skills.Where(s => s.CloudId == jwt.GetCurrentCloudId()).ToList();
            //var functionFromDB = db.Functions.Where(f => f.CloudId == jwt.GetCurrentCloudId()).ToList();
            //var equipmentsFromDB = db.Equipments.Where(e => e.CloudId == jwt.GetCurrentCloudId()).ToList();

            //contentObject.StartDate = (DateTime) projectFromDB.StartDate;
            //contentObject.Budget = (int) projectFromDB.Budget;
            //contentObject.Deadline = (int) (projectFromDB.Deadline.Value.Subtract(projectFromDB.StartDate.Value).TotalDays);
            //contentObject.TaskList =  projectFromDB.Tasks.ToList();
            //contentObject.WorkerList = workerFromDB;
            //contentObject.SkillList = skillFromDB;
            //contentObject.FunctionList = functionFromDB;
            //contentObject.EquipmentList = equipmentsFromDB;

            //var content = new StringContent(JsonSerializer.Serialize(null));
            var response = await apiMicro.Get($"/api/Algorithm/TestConverter?projectId={projectId}");
            dynamic responseContent;
            if (response.IsSuccessStatusCode)
            {
                responseContent = await response.Content.ReadFromJsonAsync<OutputFromORDTO>();
            }
            else
            {
                throw new Exception("Error");
            }
            return responseContent;

        }


        public async Task<EstimatedResultDTO> EstimateWorkforce(int projectId)
        {
            //var jwt = new JWTManagerService(http);
            //var contentObject = new InputToORDTO();

            //var projectFromDB = db.Projects .Where(p => p.CloudId == jwt.GetCurrentCloudId())
            //    .Include(p => p.Tasks).FirstOrDefault();
            //var workerFromDB = db.Workforces.Where(w => w.CloudId == jwt.GetCurrentCloudId()).ToList();
            //var skillFromDB = db.Skills.Where(s => s.CloudId == jwt.GetCurrentCloudId()).ToList();
            //var functionFromDB = db.Functions.Where(f => f.CloudId == jwt.GetCurrentCloudId()).ToList();
            //var equipmentsFromDB = db.Equipments.Where(e => e.CloudId == jwt.GetCurrentCloudId()).ToList();

            //contentObject.StartDate = (DateTime) projectFromDB.StartDate;
            //contentObject.Budget = (int) projectFromDB.Budget;
            //contentObject.Deadline = (int) (projectFromDB.Deadline.Value.Subtract(projectFromDB.StartDate.Value).TotalDays);
            //contentObject.TaskList =  projectFromDB.Tasks.ToList();
            //contentObject.WorkerList = workerFromDB;
            //contentObject.SkillList = skillFromDB;
            //contentObject.FunctionList = functionFromDB;
            //contentObject.EquipmentList = equipmentsFromDB;

            //var content = new StringContent(JsonSerializer.Serialize(null));
            var response = await apiMicro.Get($"/api/WorkforceEstimator/GetEstimateWorkforce?projectId={projectId}");
            dynamic responseContent;
            if (response.IsSuccessStatusCode)
            {
                responseContent = await response.Content.ReadFromJsonAsync<EstimatedResultDTO>();
            }
            else
            {
                throw new Exception("Error");
            }
            return responseContent;

        }
    }
}
