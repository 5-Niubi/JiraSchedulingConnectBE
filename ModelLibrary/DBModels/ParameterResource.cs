namespace ModelLibrary.DBModels
{
    public partial class ParameterResource
    {
        public int Id
        {
            get; set;
        }
        public int ParameterId
        {
            get; set;
        }
        public int ResourceId
        {
            get; set;
        }
        public string Type { get; set; } = null!;
        public DateTime? CreateDatetime
        {
            get; set;
        }
        public bool? IsDelete
        {
            get; set;
        }
        public DateTime? DeleteDatetime
        {
            get; set;
        }

        public virtual Parameter Parameter { get; set; } = null!;
        public virtual Workforce Resource { get; set; } = null!;
    }
}
