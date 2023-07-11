using System;
using AlgorithmServiceServer.DTOs.AlgorithmController;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.DBModels;
using RcpspAlgorithmLibrary;

namespace AlgorithmServiceServer.Services.Interfaces
{
	public class PertGraphValidatorService: IPertGraphValidatorService
	{


        private readonly JiraDemoContext db;
        private readonly HttpContext http;


        private DirectedGraph DirectedGraph;

        public PertGraphValidatorService(JiraDemoContext db, IHttpContextAccessor httpAccessor)
		{
            this.db = db;
            http = httpAccessor.HttpContext;
        }

        public async Task<bool> IsValidDAG(int projectId)
        {
            
            // get all task active by project id
            //var cloudId = new JWTManagerService(http).GetCurrentCloudId();
            var cloudId = "ea48ddc7-ed56-4d60-9b55-02667724849d"; // DEBUG

            var projectFromDB = await db.Projects
                .Where(p => p.CloudId == cloudId)
                .Include(p => p.Tasks)
                .FirstOrDefaultAsync();

            var skillFromDB = db.Skills
                .Where(s => s.CloudId == cloudId)
                .ToList();

            var taskFromDB = db.Tasks
                .Where(t => t.ProjectId == projectId)
                .Include(t => t.TaskPrecedenceTasks)
                .Include(t => t.TasksSkillsRequireds)
                .ToList();


            var inputToEstimator = new InputToEstimatorDTO();
            inputToEstimator.SkillList = skillFromDB;
            inputToEstimator.TaskList = taskFromDB;

            // convert from input data (db) -> input estimator's
            var converter = new EstimatorConverter(inputToEstimator);
            var outputToEstimator = converter.ToEs();


            var TaskAdjacency = outputToEstimator.TaskAdjacency;

            // validate DAG tasks in by projectid

            int startNode = 0;
            DirectedGraph = new DirectedGraph(startNode);
            //DirectedGraph.LoadData(TaskAdjacency);


            return true;
              
        }

        public Task<bool> IsValidDAG(string projectId)
        {
            throw new NotImplementedException();
        }
    }
}

