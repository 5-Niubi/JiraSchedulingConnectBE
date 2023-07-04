namespace JiraSchedulingConnectAppService.Models;

public partial class TaskPrecedence
{
    public int TaskId { get; set; }

    public int PrecedenceId { get; set; }

    public DateTime? CreateDatetime { get; set; }

    public bool? IsDelete { get; set; }

    public DateTime? DeleteDatetime { get; set; }

    public virtual Task Precedence { get; set; } = null!;

    public virtual Task Task { get; set; } = null!;
}
