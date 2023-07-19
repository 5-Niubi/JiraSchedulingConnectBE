namespace ModelLibrary.DTOs.PertSchedule
{
    public class TaskUpdatedRequest
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string? Name { get; set; }
        public double? Duration { get; set; }
        public int? MilestoneId { get; set; }

        public List<SkillRequiredDTO>? SkillRequireds { get; set; }
        public List<PrecedenceRequestDTO>? Precedences { get; set; }

    }
}

