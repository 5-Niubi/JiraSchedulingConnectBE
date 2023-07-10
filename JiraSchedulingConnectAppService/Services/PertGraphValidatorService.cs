using System;
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
            int startNode = 0;
            DirectedGraph  = new DirectedGraph(startNode);
            // get all task active by project id
            var taskFromDB = db.Tasks.Where(t => t.ProjectId == projectId & t.IsDelete == false)
                .Include(t => t.TaskPrecedenceTasks)
                .Include(t => t.TasksSkillsRequireds);
            // validate DAG tasks in by projectid

            return true;
              
        }

        public Task<bool> IsValidDAG(string projectId)
        {
            throw new NotImplementedException();
        }
    }
}

