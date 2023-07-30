using System;
using ModelLibrary.DBModels;

namespace ModelLibrary.DTOs.Permission
{
    public class PlanPermissionResponseDTO
    {
        public int Id { get; set; }
        public int? PlanSubscriptionId { get; set; }
        public string? Permission { get; set; }
        public DateTime? CreateDatetime { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? DeleteDatetime { get; set; }
    }
}

