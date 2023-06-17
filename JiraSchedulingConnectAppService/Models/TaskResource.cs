using System;
using System.Collections.Generic;

namespace JiraSchedulingConnectAppService.Models;

public partial class TaskResource
{
    public int Id { get; set; }

    public int? TaskId { get; set; }

    public int? ResourceId { get; set; }

    public string? Type { get; set; }

    public virtual Equipment? Resource { get; set; }

    public virtual Workforce? ResourceNavigation { get; set; }

    public virtual Task? Task { get; set; }
}
