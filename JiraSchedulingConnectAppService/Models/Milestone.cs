namespace JiraSchedulingConnectAppService.Models;

public partial class Milestone
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public int? ProjectId { get; set; }

    public DateTime? CreateDatetime { get; set; }

    public bool? IsDelete { get; set; }

    public DateTime? DeleteDatetime { get; set; }

    public virtual Project? Project { get; set; }

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
}
