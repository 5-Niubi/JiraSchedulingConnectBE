﻿using System;
using System.Collections.Generic;

namespace JiraSchedulingConnectAppService.Models;

public partial class Workforce
{
    public int Id { get; set; }

    public string? AccountId { get; set; }

    public string? Email { get; set; }

    public string? AccountType { get; set; }

    public string? Name { get; set; }

    public string? Avatar { get; set; }

    public string? DisplayName { get; set; }

    public int? Active { get; set; }

    public string? CloudId { get; set; }

    public double? UnitSalary { get; set; }

    public int? WorkingEffort { get; set; }

    public bool? IsDelete { get; set; }

    public DateTime? CreateDatetime { get; set; }

    public DateTime? DeleteDatetime { get; set; }

    public virtual ICollection<TaskResource> TaskResources { get; set; } = new List<TaskResource>();

    public virtual ICollection<WorkforceSkill> WorkforceSkills { get; set; } = new List<WorkforceSkill>();
}
