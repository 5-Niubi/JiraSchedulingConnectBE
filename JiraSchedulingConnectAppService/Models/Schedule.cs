using System;
using System.Collections.Generic;

namespace JiraSchedulingConnectAppService.Models;

public partial class Schedule
{
    public int Id { get; set; }

    public int? ProjectId { get; set; }

    public double? Duration { get; set; }

    public double? Cost { get; set; }

    public double? Quality { get; set; }

    public string? Tasks { get; set; }

    public string? CloudId { get; set; }

    public int? Selected { get; set; }

    public int? Since { get; set; }

    public string? AccountId { get; set; }

    public virtual Project? Project { get; set; }
}
