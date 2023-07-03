using AlgorithmServiceServer.DTOs.AlgorithmController;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RcpspAlgorithmLibrary;

namespace AlgorithmServiceServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlgorithmController : ControllerBase
    {
        [HttpPost]
        async public Task<IActionResult> TestConverter([FromBody]InputToORDTO inputTo)
        {
            var converter = new AlgorithmConverter(inputTo.StartDate,
                inputTo.Deadline, inputTo.Budget, inputTo.TaskList, inputTo.WorkerList,
                inputTo.EquipmentList, inputTo.SkillList,inputTo.FunctionList);
            return Ok(converter.ToOR());
        }
    }
}
