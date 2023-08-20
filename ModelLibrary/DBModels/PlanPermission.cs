using System;
using System.Collections.Generic;

namespace ModelLibrary.DBModels
{
    public partial class PlanPermission
    {
        public int Id { get; set; }
        public int? PlanSubscriptionId { get; set; }
        public string? Permission { get; set; }
        public DateTime? CreateDatetime { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? DeleteDatetime { get; set; }

        public virtual PlanSubscription? PlanSubscription { get; set; }
    }
}
