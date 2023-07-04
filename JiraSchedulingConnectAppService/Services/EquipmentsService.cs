using System;
using AutoMapper;
using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.DBModels;
using ModelLibrary.DTOs;

namespace JiraSchedulingConnectAppService.Services
{
    public class EquipmentsService : IEquipmentService
    {
        private readonly JiraDemoContext db;
        private readonly IMapper mapper;
        private readonly HttpContext? httpContext;

        public EquipmentsService(JiraDemoContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            db = dbContext;
            this.mapper = mapper;
            httpContext = httpContextAccessor.HttpContext;
        }

        public async Task<EquipmentDTO.Response> CreateEquipment(EquipmentDTO.Request equipmentRequest)
        {
            try
            {
                var jwt = new JWTManagerService(httpContext);
                var cloudId = jwt.GetCurrentCloudId();
                var equipment = mapper.Map<Equipment>(equipmentRequest);
                equipment.CloudId = cloudId;

                await db.Equipments.AddAsync(equipment);
                await db.SaveChangesAsync();
                var equipmentDTOResponse = mapper.Map<EquipmentDTO.Response>(equipment);
                return equipmentDTOResponse;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async System.Threading.Tasks.Task DeleteEquipment(EquipmentDTO.Request equipmentRequest)
        {
            try
            {
                var equipment = await db.Equipments.SingleOrDefaultAsync(x => x.Id == equipmentRequest.Id);
                db.Equipments.Remove(equipment);
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<List<EquipmentDTO.Response>> GetAllEquipments()
        {
            var jwt = new JWTManagerService(httpContext);
            var cloudId = jwt.GetCurrentCloudId();
            var query = await db.Equipments.ToListAsync();
            var queryDTOResponse = mapper.Map<List<EquipmentDTO.Response>>(query);
            return queryDTOResponse;
        }

        public async Task<EquipmentDTO.Request> GetEquipmentById(string equipment_id)
        {
            Equipment equipment = new Equipment();
            try
            {
                equipment = await db.Equipments.SingleOrDefaultAsync(x => x.Id.ToString() == equipment_id);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            var euipmentDTORequest = mapper.Map<EquipmentDTO.Request>(equipment);
            return euipmentDTORequest;
        }

        public async Task<EquipmentDTO.Response> UpdateEquipment(EquipmentDTO.Request equipmentRequest)
        {
            try
            {
                var equipment = mapper.Map<Equipment>(equipmentRequest);
                db.Update(equipment);
                await db.SaveChangesAsync();
                var equipmentDTOResponse = mapper.Map<EquipmentDTO.Response>(equipment);
                return equipmentDTOResponse;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}

