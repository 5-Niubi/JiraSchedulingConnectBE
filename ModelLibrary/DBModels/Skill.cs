using System;
using System.Collections.Generic;

namespace ModelLibrary.DBModels
{
    public partial class Skill
    {
        public Skill()
        {
            TasksSkillsRequireds = new HashSet<TasksSkillsRequired>();
            WorkforceSkills = new HashSet<WorkforceSkill>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public string? CloudId { get; set; }
        public DateTime? CreateDatetime { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? DeleteDatetime { get; set; }

        public virtual ICollection<TasksSkillsRequired> TasksSkillsRequireds { get; set; }
        public virtual ICollection<WorkforceSkill> WorkforceSkills { get; set; }
    }
}
