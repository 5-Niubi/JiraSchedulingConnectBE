using System;
using System.Collections.Generic;

namespace JiraSchedulingConnectAppService.Models;

public partial class TasksSkillsRequired
{
    public int TaskId { get; set; }

    public int SkillId { get; set; }

    public int? Level { get; set; }

    public DateTime? CreateDatetime { get; set; }

    public bool? IsDelete { get; set; }

    public DateTime? DeleteDatetime { get; set; }

    public virtual Skill Skill { get; set; } = null!;

    public virtual Task Task { get; set; } = null!;
}
