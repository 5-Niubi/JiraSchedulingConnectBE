using System;
using ModelLibrary.DTOs.Parameters;

namespace JiraSchedulingConnectAppService.Services.Interfaces
{
    public interface IEquipmentService
    {
        public Task<List<EquipmentDTOResponse>> GetAllEquipments();
        public Task<EquipmentDTOResponse> CreateEquipment(EquipmentDTORequest e);
        public Task<EquipmentDTOResponse> GetEquipmentById(string id);
        public Task DeleteEquipment(string id);
        public Task<EquipmentDTOResponse> UpdateEquipment(EquipmentDTORequest e);
    }
}

