using System;
using System.Collections.Generic;

namespace ModelLibrary.DBModels
{
    public partial class EquipmentsFunction
    {
        public int EquipmentId { get; set; }
        public int FunctionId { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? CreateDatetime { get; set; }
        public DateTime? DeleteDatetime { get; set; }

        public virtual Equipment Equipment { get; set; } = null!;
        public virtual Function Function { get; set; } = null!;
    }
}
