using System;
using System.Collections.Generic;

namespace JiraSchedulingConnectAppService.Models;

public partial class WorkforceSkill
{
    public int Id { get; set; }

    public int? WorkforceId { get; set; }

    public int? SkillId { get; set; }

    public int? Level { get; set; }

    public string? CloudId { get; set; }

    public virtual Skill? Skill { get; set; }

    public virtual Workforce? Workforce { get; set; }
}
