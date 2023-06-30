using System;
using System.Collections.Generic;

namespace ModelLibrary.DBModels
{
    public partial class Function
    {
        public Function()
        {
            TaskFunctions = new HashSet<TaskFunction>();
            Equipment = new HashSet<Equipment>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public string? CloudId { get; set; }

        public virtual ICollection<TaskFunction> TaskFunctions { get; set; }

        public virtual ICollection<Equipment> Equipment { get; set; }
    }
}
