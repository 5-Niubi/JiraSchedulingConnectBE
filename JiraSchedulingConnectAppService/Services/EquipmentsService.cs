using AutoMapper;
using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.DBModels;
using ModelLibrary.DTOs.Parameters;

namespace JiraSchedulingConnectAppService.Services
{
    public class EquipmentsService : IEquipmentService
    {
        private readonly WoTaasContext db;
        private readonly IMapper mapper;
        private readonly HttpContext? httpContext;

        public EquipmentsService(WoTaasContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            db = dbContext;
            this.mapper = mapper;
            httpContext = httpContextAccessor.HttpContext;
        }

        public async Task<EquipmentDTOResponse> CreateEquipment(EquipmentDTORequest equipmentRequest)
        {
            try
            {
                var jwt = new JWTManagerService(httpContext);
                var cloudId = jwt.GetCurrentCloudId();
                var equipment = mapper.Map<Equipment>(equipmentRequest);
                equipment.CloudId = cloudId;

                await db.Equipments.AddAsync(equipment);
                await db.SaveChangesAsync();
                var equipmentDTOResponse = mapper.Map<EquipmentDTOResponse>(equipment);
                return equipmentDTOResponse;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async System.Threading.Tasks.Task DeleteEquipment(string equipment_id)
        {
            try
            {
                var equipment = await db.Equipments.Where(e => e.Id.ToString() == equipment_id).FirstOrDefaultAsync();
                equipment.IsDelete = true;
                equipment.DeleteDatetime = DateTime.Now;
                db.Equipments.Update(equipment);
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<List<EquipmentDTOResponse>> GetAllEquipments()
        {
            var jwt = new JWTManagerService(httpContext);
            var cloudId = jwt.GetCurrentCloudId();
            var query = await db.Equipments.ToListAsync();
            var queryDTOResponse = mapper.Map<List<EquipmentDTOResponse>>(query);
            return queryDTOResponse;
        }

        public async Task<EquipmentDTOResponse> GetEquipmentById(string equipment_id)
        {
            try
            {
                var equipment = await db.Equipments.Where(e => e.Id.ToString() == equipment_id).FirstOrDefaultAsync();
                var equipmentResponse = mapper.Map<EquipmentDTOResponse>(equipment);
                return equipmentResponse;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<EquipmentDTOResponse> UpdateEquipment(EquipmentDTORequest equipmentRequest)
        {
            try
            {
                var equipment = mapper.Map<Equipment>(equipmentRequest);
                db.Update(equipment);
                await db.SaveChangesAsync();
                var equipmentDTOResponse = mapper.Map<EquipmentDTOResponse>(equipment);
                return equipmentDTOResponse;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}

