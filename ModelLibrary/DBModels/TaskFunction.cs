using System;
using System.Collections.Generic;

namespace ModelLibrary.DBModels
{
    public partial class TaskFunction
    {
        public int TaskId { get; set; }
        public int FunctionId { get; set; }
        public int? RequireTime { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? CreateDatetime { get; set; }
        public DateTime? DeleteDatetime { get; set; }

        public virtual Function Function { get; set; } = null!;
        public virtual Task Task { get; set; } = null!;
    }
}
