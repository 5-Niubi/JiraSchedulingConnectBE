﻿using System;
using System.Collections.Generic;

namespace JiraSchedulingConnectAppService.Models;

public partial class Project
{
    public int Id { get; set; }

    public string? ImageAvatar { get; set; }

    public string? Name { get; set; }

    public string? AccountId { get; set; }

    public DateTime? StartDate { get; set; }

    public double? Budget { get; set; }

    public string? BudgetUnit { get; set; }

    public DateTime? Deadline { get; set; }

    public double? ObjectiveTime { get; set; }

    public double? ObjectiveCost { get; set; }

    public double? ObjectiveQuality { get; set; }

    public string? CloudId { get; set; }

    public bool? IsDelete { get; set; }

    public DateTime? CreateDatetime { get; set; }

    public DateTime? DeleteDatetime { get; set; }

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
}
