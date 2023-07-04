namespace JiraSchedulingConnectAppService.Models;

public partial class Function
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? CloudId { get; set; }

    public bool? IsDelete { get; set; }

    public DateTime? CreateDatetime { get; set; }

    public DateTime? DeleteDatetime { get; set; }

    public virtual ICollection<EquipmentsFunction> EquipmentsFunctions { get; set; } = new List<EquipmentsFunction>();

    public virtual ICollection<TaskFunction> TaskFunctions { get; set; } = new List<TaskFunction>();
}
