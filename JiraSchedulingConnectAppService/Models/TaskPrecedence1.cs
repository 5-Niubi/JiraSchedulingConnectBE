using System;
using System.Collections.Generic;

namespace JiraSchedulingConnectAppService.Models;

public partial class TaskPrecedence1
{
    public int Id { get; set; }

    public int? TaskId { get; set; }

    public int? PrecedenceId { get; set; }

    public virtual Task? Precedence { get; set; }

    public virtual Task? Task { get; set; }
}
