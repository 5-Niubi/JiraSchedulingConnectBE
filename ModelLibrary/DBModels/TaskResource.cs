using System;
using System.Collections.Generic;

namespace ModelLibrary.DBModels
{
    public partial class TaskResource
    {
        public int TaskId { get; set; }
        public int ParameterResourceId { get; set; }
        public DateTime? CreateDatetime { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? DeleteDatetime { get; set; }

        public virtual ParameterResource ParameterResource { get; set; } = null!;
        public virtual Task Task { get; set; } = null!;
    }
}
