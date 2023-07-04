using System;
using System.Collections.Generic;

namespace ModelLibrary.DBModels
{
    public partial class TaskResource
    {
        public int TaskId { get; set; }
        public int ProjectResourceId { get; set; }
        public DateTime? CreateDatetime { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? DeleteDatetime { get; set; }

        public virtual ProjectResource ProjectResource { get; set; } = null!;
        public virtual Task Task { get; set; } = null!;
    }
}
