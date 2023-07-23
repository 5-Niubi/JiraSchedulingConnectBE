using System;
using System.Collections.Generic;

namespace ModelLibrary.DBModels
{
    public partial class PlanSubscription
    {
        public PlanSubscription()
        {
            Subscriptions = new HashSet<Subscription>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public double? Price { get; set; }
        public int? Duration { get; set; }
        public DateTime? CreateDatetime { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? DeleteDatetime { get; set; }

        public virtual ICollection<Subscription> Subscriptions { get; set; }
    }
}
