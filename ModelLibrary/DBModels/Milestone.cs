using System;
using System.Collections.Generic;

namespace ModelLibrary.DBModels
{
    public partial class Milestone
    {
        public Milestone()
        {
            Tasks = new HashSet<Task>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public int? ProjectId { get; set; }
        public DateTime? CreateDatetime { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? DeleteDatetime { get; set; }

        public virtual Project? Project { get; set; }
        public virtual ICollection<Task> Tasks { get; set; }
    }
}
