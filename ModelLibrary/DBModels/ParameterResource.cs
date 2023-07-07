namespace ModelLibrary.DBModels
{
    public partial class ParameterResource
    {
        public ParameterResource()
        {
            TaskResources = new HashSet<TaskResource>();
        }

        public int Id { get; set; }
        public int ParameterId { get; set; }
        public int ResourceId { get; set; }
        public string Type { get; set; } = null!;
        public DateTime? CreateDatetime { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? DeleteDatetime { get; set; }

        public virtual Parameter Parameter { get; set; } = null!;
        public virtual Equipment Resource { get; set; } = null!;
        public virtual Workforce ResourceNavigation { get; set; } = null!;
        public virtual ICollection<TaskResource> TaskResources { get; set; }
    }
}
