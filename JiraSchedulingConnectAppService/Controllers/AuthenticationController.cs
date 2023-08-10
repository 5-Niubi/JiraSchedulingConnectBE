using JiraSchedulingConnectAppService.Services;
using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLibrary.DBModels;
using ModelLibrary.DTOs;
using UtilsLibrary.Exceptions;
namespace JiraSchedulingConnectAppService.Controllers
{
    //[ApiController]
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
        async public Task<IActionResult> Callback(string? code, string? state, string? error, string? error_description)
        {
            try
            {
                var responeAccessible = await authenticationService.InitAuthen(code, state, error, error_description);
                return View("/Views/GrantSuccess.cshtml");
            }
            catch (UnAuthorizedException ex)
            {
                ViewData["errmsg"] = ex.Message;
                return View("/Views/Error.cshtml");
            }
            catch (Exception ex)
            {
                var responseMsg = new ResponseMessageDTO(ex.Message);
                ViewData["errmsg"] = ex.Message;
                return View("/Views/Error.cshtml");
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
