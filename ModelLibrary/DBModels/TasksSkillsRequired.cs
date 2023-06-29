using System;
using System.Collections.Generic;

namespace ModelLibrary.DBModels
{
    public partial class TasksSkillsRequired
    {
        public int TaskId { get; set; }
        public int SkillId { get; set; }
        public int? Level { get; set; }
        public string CloudId { get; set; } = null!;
        public DateTime? CreateDatetime { get; set; }
        public DateTime? IsDelete { get; set; }
        public DateTime? DeleteDatetime { get; set; }

        public virtual Skill Skill { get; set; } = null!;
        public virtual Task Task { get; set; } = null!;
    }
}
