using System;
using System.Collections.Generic;

namespace ModelLibrary.DBModels
{
    public partial class Subscription
    {
        public int Id { get; set; }
        public int? AtlassianTokenId { get; set; }
        public string? Token { get; set; }
        public int? PlanId { get; set; }
        public DateTime? CurrentPeriodStart { get; set; }
        public DateTime? CurrentPeriodEnd { get; set; }
        public DateTime? CancelAt { get; set; }
        public DateTime? CreateDatetime { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? DeleteDatetime { get; set; }

        public virtual AtlassianToken? AtlassianToken { get; set; }
        public virtual PlanSubscription? Plan { get; set; }
    }
}
