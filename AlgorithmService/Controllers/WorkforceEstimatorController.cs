using Microsoft.AspNetCore.Mvc;
using Monster;
namespace AlgorithmService.Controllers
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
                                                    
    

       List<List<int>> TaskDuration = new List<List<int>>()
        {
            new List<int> {0, 10, 1, 0}
        };


        private readonly ILogger<WorkforceEstimatorController> _logger;

        public WorkforceEstimatorController(ILogger<WorkforceEstimatorController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetEstimateWorkforce")]
        public IEnumerable<WeatherForecast> Get()
        {
            ScheduleEstimator estimator = new ScheduleEstimator(
                TaskDuration, 
                TaskExper, 
                TaskAdjacency
                );
            estimator.ForwardMethod();
            List<List<int>> outputs = estimator.Fit();
            
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = "ahihi"
            })
            .ToArray();
        }
    }
}