namespace ModelLibrary.DBModels
{
    public partial class Role
    {
        public int Id { get; set; }
        public string? CloudId { get; set; }
        public string? Name { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? CreateDatetime { get; set; }
        public DateTime? DeleteDatetime { get; set; }
    }
}
