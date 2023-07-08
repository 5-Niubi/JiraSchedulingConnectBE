using System;
using System.Collections.Generic;

namespace ModelLibrary.DBModels
{
    public partial class ScheduleTask
    {
        public ScheduleTask()
        {
            ScheduleTaskResources = new HashSet<ScheduleTaskResource>();
        }

        public int Id { get; set; }
        public int? ScheduleId { get; set; }
        public int? TaskId { get; set; }
        public DateTime? Startdate { get; set; }
        public DateTime? Enddate { get; set; }
        public DateTime? CreateDatetime { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? DeleteDatetime { get; set; }

        public virtual Schedule? Schedule { get; set; }
        public virtual Task? Task { get; set; }
        public virtual ICollection<ScheduleTaskResource> ScheduleTaskResources { get; set; }
    }
}
