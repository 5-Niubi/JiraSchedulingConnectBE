using System;
using System.Collections.Generic;

namespace ModelLibrary.DBModels
{
    public partial class ScheduleTaskResource
    {
        public int ScheduleTaskId { get; set; }
        public int ResourceId { get; set; }
        public string Type { get; set; } = null!;
        public DateTime? CreateDatetime { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? DeleteDatetime { get; set; }

        public virtual Equipment Resource { get; set; } = null!;
        public virtual Workforce ResourceNavigation { get; set; } = null!;
        public virtual ScheduleTask ScheduleTask { get; set; } = null!;
    }
}
