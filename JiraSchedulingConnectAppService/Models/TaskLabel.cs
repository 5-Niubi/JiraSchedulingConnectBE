using System;
using System.Collections.Generic;

namespace JiraSchedulingConnectAppService.Models;

public partial class TaskLabel
{
    public int Id { get; set; }

    public int? TaskId { get; set; }

    public int? LabelId { get; set; }

    public int? CloudId { get; set; }

    public DateTime? CreateDatetime { get; set; }

    public bool? IsDelete { get; set; }

    public DateTime? DeleteDatetime { get; set; }

    public virtual Label? Label { get; set; }

    public virtual Task? Task { get; set; }
}
