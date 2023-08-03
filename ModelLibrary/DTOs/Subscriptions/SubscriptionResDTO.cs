namespace ModelLibrary.DTOs.Subscriptions
{
    public class SubscriptionResDTO
    {
        public string? Token
        {
            get; set;
        }
        public PlanSubscriptionResDTO? Plan
        {
            get; set;
        }
        public DateTime? CurrentPeriodStart
        {
            get; set;
        }
        public DateTime? CurrentPeriodEnd
        {
            get; set;
        }
        public DateTime? CreateDatetime
        {
            get; set;
        }
    }
}
