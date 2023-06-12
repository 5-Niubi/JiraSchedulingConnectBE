using System;
using System.Collections.Generic;

namespace JiraSchedulingConnectAppService.Models;

public partial class AtlassianToken
{
    public int Id { get; set; }

    public string? AccountId { get; set; }

    public string? CloudId { get; set; }

    public string? AccessToken { get; set; }

    public string? RefressToken { get; set; }
}
