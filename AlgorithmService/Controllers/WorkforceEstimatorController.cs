using AlgorithmServiceServer;
using Microsoft.AspNetCore.Mvc;
using RcpspEstimator;

namespace AlgorithmServiceServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WorkforceEstimatorController : ControllerBase
    {

        List<List<int>> TaskAdjacency = new List<List<int>>()
                                                {
                                                    new List<int> {0, 0, 0, 0, 0},
                                                    new List<int> {1, 0, 0, 0, 0},
                                                    new List<int> {0, 1, 0, 0, 0},
                                                    new List<int> {0, 1, 0, 0, 0},
                                                    new List<int> {0, 0, 1, 1, 0}
                                                };

        List<List<int>> TaskExper = new List<List<int>>()
        {
            new List<int> {0,0,0},
            new List<int> {1,0,2},
            new List<int> {3,0,3},
            new List<int> {2,0,4},
            new List<int> {0,0,0}
        };


        List<int> TaskDuration = new List<int> { 0, 10, 1, 3, 1 };

        private readonly ILogger<WorkforceEstimatorController> _logger;

        public WorkforceEstimatorController(ILogger<WorkforceEstimatorController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetEstimateWorkforce")]
        public IEnumerable<EstimatedWorkforce> Get()
        {

            // TODO
            // Get data from relate tables: Task
            // Encode to matrix TaskDuration, TaskExper, TaskAdjacency

            ScheduleEstimator estimator = new ScheduleEstimator(
                TaskDuration,
                TaskExper,
                TaskAdjacency
                );
            estimator.ForwardMethod();
            List<List<int>> outputs = estimator.Fit();


            // TODO
            // Store output to Database

            return Enumerable.Range(0, outputs.Count).Select(index => new EstimatedWorkforce
            {
                Id = 123,
                Skills = outputs[index],
                NumberOfWorkforce = 2
            })
            .ToArray();


        }
    }
}