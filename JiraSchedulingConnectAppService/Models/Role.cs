using System;
using System.Collections.Generic;

namespace JiraSchedulingConnectAppService.Models;

public partial class Role
{
    public int Id { get; set; }

    public string? CloudId { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<AccountRole> AccountRoles { get; set; } = new List<AccountRole>();
}
