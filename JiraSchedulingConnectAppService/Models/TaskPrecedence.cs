using System;
using System.Collections.Generic;

namespace JiraSchedulingConnectAppService.Models;

public partial class TaskPrecedence
{
    public int Id { get; set; }

    public int? TaskId { get; set; }

    public int? PrecedenceId { get; set; }

    public DateTime? CreateDatetime { get; set; }

    public bool? IsDelete { get; set; }

    public DateTime? DeleteDatetime { get; set; }

    public virtual Task? Precedence { get; set; }

    public virtual Task? Task { get; set; }
}
