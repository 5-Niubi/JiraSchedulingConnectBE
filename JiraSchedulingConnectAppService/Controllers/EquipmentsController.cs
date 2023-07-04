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

namespace JiraSchedulingConnectAppService.Controllers
{
    [Route("api/[controller]")]
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
                return BadRequest();
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEquipmentById(string id)
        {
            var response = EquipmentService.GetEquipmentById(id);
            if (response == null)
            {
                return NotFound();
            }
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEquipment(string id, EquipmentDTO.Request equipmentRequest)
        {
            if (id != equipmentRequest.Id.ToString())
            {
                return BadRequest();
            }
            try
            {
                await EquipmentService.UpdateEquipment(equipmentRequest);
                var equipment = EquipmentService.GetEquipmentById(id);
                return Ok(equipment);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateEquipment([FromBody]EquipmentDTO.Request equipmentRequest)
        {
            try
            {
                return Ok(await EquipmentService.CreateEquipment(equipmentRequest));
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEquipment(string id)
        {
            try
            {
                var equipmentDTORequest = EquipmentService.GetEquipmentById(id);
                if (equipmentDTORequest == null)
                {
                    return NotFound();
                }
                await EquipmentService.DeleteEquipment(await equipmentDTORequest);
                return Ok("Delete success");
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
    }
}
