using JiraSchedulingConnectAppService.Services;
using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using ModelLibrary.DBModels;
using ModelLibrary.DTOs;

namespace JiraSchedulingConnectAppService.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly AuthenticationService authenticationService;
        private readonly ILoggerService _Logger;
        public AuthenticationController(JiraDemoContext db, IConfiguration config, ILoggerService logger)
        {
            this._Logger = logger;
            authenticationService = new AuthenticationService(db, config);
        }

        [HttpGet]
        async public Task<IActionResult> Callback(string code, string state)
        {
            try
            {
                var responeAccessible = await authenticationService.InitAuthen(code, state);
                return Ok(responeAccessible);
            }
            catch (Exception ex)
            {
                this._Logger.Log(LogLevel.Error, ex);
                var responseMsg = new ResponseMessageDTO(ex.Message);
                return Unauthorized(responseMsg);
            }
        }


    }
}
