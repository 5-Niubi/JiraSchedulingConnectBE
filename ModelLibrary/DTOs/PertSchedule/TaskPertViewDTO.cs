namespace ModelLibrary.DTOs.PertSchedule
{
    public class TaskPertViewDTO
    {
        public int Id
        {
            get; set;
        }
        public string? Name
        {
            get; set;
        }
        public double? Duration
        {
            get; set;
        }
        public int? MilestoneId
        {
            get; set;
        }

        public List<TaskPrecedenceDTO>? Precedences
        {
            get; set;
        }
        public List<SkillRequiredDTO>? SkillRequireds
        {
            get; set;
        }

    }
}

