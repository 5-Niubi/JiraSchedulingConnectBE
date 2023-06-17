using System;
using System.Collections.Generic;

namespace JiraSchedulingConnectAppService.Models;

public partial class TaskPrecedence
{
    public int Id { get; set; }

    public int? TaskId { get; set; }

    public int? PrecedenceTaskId { get; set; }
}
