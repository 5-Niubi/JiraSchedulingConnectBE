using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLibrary.DTOs;
using ModelLibrary.DTOs.Permission;

namespace JiraSchedulingConnectAppService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class PermissionController : ControllerBase
    {


        private IPermissionService permissionService;
        private readonly ILoggerManager _Logger;

        public PermissionController(IPermissionService permissionService, ILoggerManager logger)

        {
            _Logger = logger;
            this.permissionService = permissionService;


        }

        [HttpPost]
        public async Task<IActionResult> AttachPlanPermission(AttachPlanPermissionRequestDTO AttachPlanPermissionRequest)
        {
            try
            {
                var response = await permissionService.AttachPlanPermission(AttachPlanPermissionRequest);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex.Message);
                var response = new ResponseMessageDTO(ex.Message);
                return BadRequest(response);
            }
        }



    }
}

