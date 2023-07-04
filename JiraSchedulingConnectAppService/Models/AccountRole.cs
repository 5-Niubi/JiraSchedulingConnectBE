namespace JiraSchedulingConnectAppService.Models;

public partial class AccountRole
{
    public string? AccountId { get; set; }

    public int Id { get; set; }

    public int? TokenId { get; set; }

    public bool? IsDelete { get; set; }

    public DateTime? CreateDatetime { get; set; }

    public bool? IsDelete1 { get; set; }

    public DateTime? DeleteDatetime { get; set; }

    public virtual AtlassianToken? Token { get; set; }
}
