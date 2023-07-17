using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JiraSchedulingConnectAppService.Services;
using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.DBModels;
using ModelLibrary.DTOs;
using ModelLibrary.DTOs.Parameters;

namespace JiraSchedulingConnectAppService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class EquipmentsController : ControllerBase
    {
        private IEquipmentService EquipmentService;
        public EquipmentsController(IEquipmentService equipmentService)
        {
            this.EquipmentService = equipmentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEquipments()
        {
            try
            {
                var response = await EquipmentService.GetAllEquipments();
                return Ok(response);
            }
            catch (Exception ex)
            {
                
                var response = new ResponseMessageDTO(ex.Message);
                return BadRequest(response);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetEquipmentById(string id)
        {
            try
            {
                var response = await EquipmentService.GetEquipmentById(id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                
                var response = new ResponseMessageDTO(ex.Message);
                return BadRequest(response);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateEquipment(string id, EquipmentDTORequest equipmentRequest)
        {
            try
            {
                await EquipmentService.UpdateEquipment(equipmentRequest);
                var equipment = EquipmentService.GetEquipmentById(id);
                return Ok(equipment);
            }
            catch (Exception ex)
            {
                
                var response = new ResponseMessageDTO(ex.Message);
                return BadRequest(response);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateEquipment([FromBody]EquipmentDTORequest equipmentRequest)
        {
            try
            {
                return Ok(await EquipmentService.CreateEquipment(equipmentRequest));
            }
            catch (Exception ex)
            {
                
                var response = new ResponseMessageDTO(ex.Message);
                return BadRequest(response);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteEquipment(string id)
        {
            try
            {
                var equipmentDTORequest = EquipmentService.GetEquipmentById(id);
                await EquipmentService.DeleteEquipment(id);
                return Ok("Delete success");
            }
            catch (Exception ex)
            {
                
                var response = new ResponseMessageDTO(ex.Message);
                return BadRequest(response);
            }
        }
    }
}
