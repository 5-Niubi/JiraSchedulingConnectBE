namespace ModelLibrary.DTOs.Projects
{
    public class ProjectListHomePageDTO
    {
        public int Id { get; set; }

        public string? ImageAvatar { get; set; }

        public string? Name { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? Deadline { get; set; }

        public DateTime? CreateDatetime { get; set; }

        public int TaskCount { get; set; }
    }
}
