using System;
using System.Collections.Generic;

namespace ModelLibrary.DBModels
{
    public partial class Task
    {
        public object TaskPrecesdences;

        public Task()
        {
            TaskFunctions = new HashSet<TaskFunction>();
            TaskPrecedencePrecedences = new HashSet<TaskPrecedence>();
            TaskPrecedenceTasks = new HashSet<TaskPrecedence>();
            TaskResources = new HashSet<TaskResource>();
            TasksSkillsRequireds = new HashSet<TasksSkillsRequired>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public double? Duration { get; set; }
        public string? CloudId { get; set; }
        public int? ProjectId { get; set; }
        public int? MilestoneId { get; set; }
        public DateTime? CreateDatetime { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? DeleteDatetime { get; set; }

        public virtual Milestone? Milestone { get; set; }
        public virtual Project? Project { get; set; }
        public virtual ICollection<TaskFunction> TaskFunctions { get; set; }
        public virtual ICollection<TaskPrecedence> TaskPrecedencePrecedences { get; set; }
        public virtual ICollection<TaskPrecedence> TaskPrecedenceTasks { get; set; }
        public virtual ICollection<TaskResource> TaskResources { get; set; }
        public virtual ICollection<TasksSkillsRequired> TasksSkillsRequireds { get; set; }
    }
}
