using System;
using System.Collections.Generic;

namespace ModelLibrary.DBModels
{
    public partial class Equipment
    {
        public Equipment()
        {
            TaskResources = new HashSet<TaskResource>();
            Functions = new HashSet<Function>();
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

        public virtual ICollection<TaskResource> TaskResources { get; set; }

        public virtual ICollection<Function> Functions { get; set; }
    }
}
