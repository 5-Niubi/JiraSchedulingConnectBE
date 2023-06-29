using System;
using System.Collections.Generic;

namespace ModelLibrary.DBModels
{
    public partial class TaskResource
    {
        public TaskResource()
        {
            Functions = new HashSet<Function>();
        }

        public int Id { get; set; }
        public int? TaskId { get; set; }
        public int? ResourceId { get; set; }
        public string? Type { get; set; }
        public DateTime? CreateDatetime { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? DeleteDatetime { get; set; }

        public virtual Equipment? Resource { get; set; }
        public virtual Workforce? ResourceNavigation { get; set; }
        public virtual Task? Task { get; set; }

        public virtual ICollection<Function> Functions { get; set; }
    }
}
