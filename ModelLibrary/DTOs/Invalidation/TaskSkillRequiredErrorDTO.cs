using ModelLibrary.DTOs.PertSchedule;

namespace ModelLibrary.DTOs.Invalidator
{


    public class TaskSkillRequiredErrorDTO
    {
        public int TaskId { get; set; }
        public SkillRequiredDTO SkillRequired { get; set; }
        public String Messages { get; set; }
    }
}

