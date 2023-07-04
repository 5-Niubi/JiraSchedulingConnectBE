using System;
using ModelLibrary.DTOs;

namespace JiraSchedulingConnectAppService.Services.Interfaces
{
    public interface IEquipmentService
    {
        public Task<List<EquipmentDTO.Response>> GetAllEquipments();
        public Task<EquipmentDTO.Response> CreateEquipment(EquipmentDTO.Request e);
        public Task<EquipmentDTO.Request> GetEquipmentById(string equipment_id);
        public Task DeleteEquipment(EquipmentDTO.Request e);
        public Task<EquipmentDTO.Response> UpdateEquipment(EquipmentDTO.Request e);
    }
}

