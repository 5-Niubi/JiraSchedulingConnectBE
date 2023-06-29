﻿using System;
using System.Collections.Generic;

namespace JiraSchedulingConnectAppService.Models;

public partial class Label
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public int? CloudId { get; set; }

    public DateTime? CreateDatetime { get; set; }

    public bool? IsDelete { get; set; }

    public DateTime? DeleteDatetime { get; set; }

    public virtual ICollection<TaskLabel> TaskLabels { get; set; } = new List<TaskLabel>();
}
