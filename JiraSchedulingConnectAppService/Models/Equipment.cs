using System;
using System.Collections.Generic;

namespace JiraSchedulingConnectAppService.Models;

public partial class Equipment
{
    public int Id { get; set; }

    public string? CloudId { get; set; }

    public string Name { get; set; } = null!;

    public int? Quantity { get; set; }

    public string? Unit { get; set; }

    public double? UnitPrice { get; set; }

    public DateTime? CreateDatetime { get; set; }

    public bool? IsDelete { get; set; }

    public DateTime? DeleteDatetime { get; set; }

    public virtual ICollection<EquipmentsFunction> EquipmentsFunctions { get; set; } = new List<EquipmentsFunction>();

    public virtual ICollection<TaskResource> TaskResources { get; set; } = new List<TaskResource>();
}
