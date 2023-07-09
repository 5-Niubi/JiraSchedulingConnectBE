using System;
using ModelLibrary.DTOs;

namespace JiraSchedulingConnectAppService.Services.Interfaces
{
    public interface IEquipmentService
    {
        public Task<List<EquipmentDTO.Response>> GetAllEquipments();
        public Task<EquipmentDTO.Response> CreateEquipment(EquipmentDTO.Request e);
        public Task<EquipmentDTO.Response> GetEquipmentById(string id);
        public Task DeleteEquipment(string id);
        public Task<EquipmentDTO.Response> UpdateEquipment(EquipmentDTO.Request e);
    }
}

