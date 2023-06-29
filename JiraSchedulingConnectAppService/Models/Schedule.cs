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

    public DateTime? Since { get; set; }

    public string? AccountId { get; set; }

    public bool? IsDelete { get; set; }

    public DateTime? CreateDatetime { get; set; }

    public DateTime? DeleteDatetime { get; set; }

    public virtual Project? Project { get; set; }
}
