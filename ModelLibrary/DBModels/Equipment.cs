using System;
using System.Collections.Generic;

namespace ModelLibrary.DBModels
{
    public partial class Equipment
    {
        public Equipment()
        {
            EquipmentsFunctions = new HashSet<EquipmentsFunction>();
            ParameterResources = new HashSet<ParameterResource>();
        }

        public int Id { get; set; }
        public string? CloudId { get; set; }
        public string Name { get; set; } = null!;
        public int? Quantity { get; set; }
        public string? Unit { get; set; }
        public double? UnitPrice { get; set; }
        public DateTime? CreateDatetime { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? DeleteDatetime { get; set; }

        public virtual ICollection<EquipmentsFunction> EquipmentsFunctions { get; set; }
        public virtual ICollection<ParameterResource> ParameterResources { get; set; }
    }
}