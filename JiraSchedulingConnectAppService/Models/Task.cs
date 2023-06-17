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

    public virtual Project? Project { get; set; }

    public virtual ICollection<TaskLabel> TaskLabels { get; set; } = new List<TaskLabel>();

    public virtual ICollection<TaskPrecedence1> TaskPrecedence1Precedences { get; set; } = new List<TaskPrecedence1>();

    public virtual ICollection<TaskPrecedence1> TaskPrecedence1Tasks { get; set; } = new List<TaskPrecedence1>();

    public virtual ICollection<TaskResource> TaskResources { get; set; } = new List<TaskResource>();
}
