using System;
using System.Collections.Generic;

namespace JiraSchedulingConnectAppService.Models;

public partial class Skill
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? CloudId { get; set; }

    public DateTime? CreateDatetime { get; set; }

    public bool? IsDelete { get; set; }

    public DateTime? DeleteDatetime { get; set; }

    public virtual ICollection<TasksSkillsRequired> TasksSkillsRequireds { get; set; } = new List<TasksSkillsRequired>();

    public virtual ICollection<WorkforceSkill> WorkforceSkills { get; set; } = new List<WorkforceSkill>();
}
