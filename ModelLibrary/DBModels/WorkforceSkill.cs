using System;
using System.Collections.Generic;

namespace ModelLibrary.DBModels
{
    public partial class WorkforceSkill
    {
        public int WorkforceId { get; set; }
        public int SkillId { get; set; }
        public int? Level { get; set; }
        public DateTime? CreateDatetime { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? DeleteDatetime { get; set; }
        public string? CloudId { get; set; }

        public virtual Skill Skill { get; set; } = null!;
        public virtual Workforce Workforce { get; set; } = null!;
    }
}
