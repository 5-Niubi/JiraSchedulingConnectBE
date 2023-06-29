using System;
using System.Collections.Generic;

namespace ModelLibrary.DBModels
{
    public partial class Function
    {
        public Function()
        {
            Equipment = new HashSet<Equipment>();
            Tasks = new HashSet<TaskResource>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public string? CloudId { get; set; }

        public virtual ICollection<Equipment> Equipment { get; set; }
        public virtual ICollection<TaskResource> Tasks { get; set; }
    }
}
