using System;
using System.Collections.Generic;

namespace ModelLibrary.DBModels
{
    public partial class ProjectResource
    {
        public ProjectResource()
        {
            TaskResources = new HashSet<TaskResource>();
        }

        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int ResourceId { get; set; }
        public string Type { get; set; } = null!;
        public DateTime? CreateDatetime { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? DeleteDatetime { get; set; }

        public virtual Project Project { get; set; } = null!;
        public virtual Equipment Resource { get; set; } = null!;
        public virtual Workforce ResourceNavigation { get; set; } = null!;
        public virtual ICollection<TaskResource> TaskResources { get; set; }
    }
}
