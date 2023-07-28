using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLibrary.DBModels;
using ModelLibrary.DTOs;

namespace JiraSchedulingConnectAppService.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class AuthenticationController : Controller
    {
        private readonly IAuthenticationService authenticationService;
        public AuthenticationController(JiraDemoContext db, IConfiguration config
            , IAuthenticationService authenticationService)
        {
            this.authenticationService = authenticationService;
        }

        [HttpGet]
        async public Task<IActionResult> Callback(string code, string state)
        {
            try
            {
                var responeAccessible = await authenticationService.InitAuthen(code, state);
                return View("/Views/GrantSuccess");
            }
            catch (Exception ex)
            {
                
                var responseMsg = new ResponseMessageDTO(ex.Message);
                return Unauthorized(responseMsg);
            }
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetTokenForDownload()
        {
            try
            {
                var responeAccessible = authenticationService.GetTokenForHandshakeDownload();
                return Ok(responeAccessible);
            }
            catch (Exception ex)
            {
                var responseMsg = new ResponseMessageDTO(ex.Message);
                return BadRequest(responseMsg);
            }
        }
    }
}
