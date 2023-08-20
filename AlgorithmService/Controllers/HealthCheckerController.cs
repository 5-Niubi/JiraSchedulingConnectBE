using System;
using AlgorithmServiceServer.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLibrary.DTOs;
using UtilsLibrary.Exceptions;

namespace AlgorithmServiceServer.Controllers
{

    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class HealthCheckerController : ControllerBase
    {
        public HealthCheckerController()
        {
        }

        [HttpGet]
        async public Task<IActionResult> ready()
        {
            return Ok();
        }

    }
    
}

