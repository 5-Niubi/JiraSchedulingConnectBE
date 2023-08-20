namespace ModelLibrary.DTOs.PertSchedule
{
    public class TaskPertDetailDTO
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

        public DateTime? CreateDatetime
        {
            get; set;
        }
        public DateTime? DeleteDatetime
        {
            get; set;
        }

        public List<int>? RequiredSkills
        {
            get; set;
        }
        public List<int>? PredenceTasks
        {
            get; set;
        }

    }
}

