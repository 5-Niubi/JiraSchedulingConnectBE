using System;
using System.Collections.Generic;

namespace JiraSchedulingConnectAppService.Models;

public partial class AccountRole
{
    public string? AccountId { get; set; }

    public int Id { get; set; }

    public int? TokenId { get; set; }

    public virtual AtlassianToken? Token { get; set; }
}
