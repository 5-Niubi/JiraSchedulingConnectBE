namespace ModelLibrary.DBModels
{
    public partial class Project
    {
        public Project()
        {
            Milestones = new HashSet<Milestone>();
            Parameters = new HashSet<Parameter>();
            Tasks = new HashSet<Task>();
        }

        public int Id
        {
            get; set;
        }
        public string? ImageAvatar
        {
            get; set;
        }
        public string? Name
        {
            get; set;
        }
        public string? AccountId
        {
            get; set;
        }
        public DateTime? StartDate
        {
            get; set;
        }
        public double? Budget
        {
            get; set;
        }
        public string? BudgetUnit
        {
            get; set;
        }
        public DateTime? Deadline
        {
            get; set;
        }
        public double? ObjectiveTime
        {
            get; set;
        }
        public double? ObjectiveCost
        {
            get; set;
        }
        public double? ObjectiveQuality
        {
            get; set;
        }
        public double? BaseWorkingHour
        {
            get; set;
        }
        public string? CloudId
        {
            get; set;
        }
        public bool? IsDelete
        {
            get; set;
        }
        public DateTime? CreateDatetime
        {
            get; set;
        }
        public DateTime? DeleteDatetime
        {
            get; set;
        }

        public virtual ICollection<Milestone> Milestones
        {
            get; set;
        }
        public virtual ICollection<Parameter> Parameters
        {
            get; set;
        }
        public virtual ICollection<Task> Tasks
        {
            get; set;
        }
    }
}
