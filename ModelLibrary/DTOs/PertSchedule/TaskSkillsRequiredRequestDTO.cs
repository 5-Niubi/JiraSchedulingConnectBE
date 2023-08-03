namespace ModelLibrary.DTOs.PertSchedule
{
    public class TaskSkillsRequiredRequestDTO
    {
        public int TaskId
        {
            get; set;
        }
        public List<SkillRequiredRequestDTO> SkillsRequireds
        {
            get; set;
        }
    }
}

