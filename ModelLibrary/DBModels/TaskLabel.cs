using System;
using System.Collections.Generic;

namespace ModelLibrary.DBModels
{
    public partial class TaskLabel
    {
        public int TaskId { get; set; }
        public int LabelId { get; set; }
        public DateTime? CreateDatetime { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? DeleteDatetime { get; set; }

        public virtual Label Label { get; set; } = null!;
        public virtual Task Task { get; set; } = null!;
    }
}
