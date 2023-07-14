namespace ModelLibrary.DTOs.Skills
{
    public class SkillDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? CloudId { get; set; }
        public int? Level { get; set; }
        public DateTime? CreateDatetime { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? DeleteDatetime { get; set; }
    }
}

