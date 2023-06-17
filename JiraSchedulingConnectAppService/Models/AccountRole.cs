using System;
using System.Collections.Generic;

namespace JiraSchedulingConnectAppService.Models;

public partial class AccountRole
{
    public int? RoleId { get; set; }

    public string? AccountId { get; set; }

    public int Id { get; set; }

    public virtual Role? Role { get; set; }
}
