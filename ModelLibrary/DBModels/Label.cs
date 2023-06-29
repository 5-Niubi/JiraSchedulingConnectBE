using System;
using System.Collections.Generic;

namespace ModelLibrary.DBModels
{
    public partial class Label
    {
        public Label()
        {
            TaskLabels = new HashSet<TaskLabel>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public int? CloudId { get; set; }
        public DateTime? CreateDatetime { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? DeleteDatetime { get; set; }

        public virtual ICollection<TaskLabel> TaskLabels { get; set; }
    }
}
