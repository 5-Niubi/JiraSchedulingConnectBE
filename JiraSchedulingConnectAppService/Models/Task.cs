using System;
using System.Collections.Generic;

namespace JiraSchedulingConnectAppService.Models;

public partial class Task
{
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

    public virtual ICollection<TaskFunction> TaskFunctions { get; set; } = new List<TaskFunction>();

    public virtual ICollection<TaskPrecedence> TaskPrecedencePrecedences { get; set; } = new List<TaskPrecedence>();

    public virtual ICollection<TaskPrecedence> TaskPrecedenceTasks { get; set; } = new List<TaskPrecedence>();

    public virtual ICollection<TaskResource> TaskResources { get; set; } = new List<TaskResource>();

    public virtual ICollection<TasksSkillsRequired> TasksSkillsRequireds { get; set; } = new List<TasksSkillsRequired>();
}
