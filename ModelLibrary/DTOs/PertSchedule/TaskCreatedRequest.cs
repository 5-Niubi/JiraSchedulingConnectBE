namespace ModelLibrary.DTOs.PertSchedule
{
    public class TaskCreatedRequest
    {

        public int ProjectId { get; set; }
        public string Name { get; set; }
        public double Duration { get; set; }
        public int MilestoneId { get; set; }

        public List<SkillRequiredRequestDTO>? SkillRequireds { get; set; }
        public List<PrecedenceRequestDTO>? Precedences { get; set; }

    }
}

