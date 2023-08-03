using ModelLibrary.DTOs.Parameters;
using ModelLibrary.DTOs.PertSchedule;

namespace ModelLibrary.DTOs.Invalidator
{


    public class TaskSkillRequiredErrorDTO
    {
        public int TaskId
        {
            get; set;
        }
        public SkillRequiredDTO SkillRequired
        {
            get; set;
        }
        public String Messages
        {
            get; set;
        }
    }


    public class TaskSkillsRequiredErrorDTO
    {
        public int TaskId
        {
            get; set;
        }
        public List<SkillRequiredDTO> SkillRequireds
        {
            get; set;
        }
        public String Messages
        {
            get; set;
        }
    }
    public class ParamsErrorWithRecommendDTO
    {
        public List<TaskSkillRequiredErrorDTO> TaskSkillRequiredError
        {
            get; set;
        }
        public List<RecomendWorkforceTaskParams> RecomendWorkforces
        {
            get; set;
        }
        public String Messages
        {
            get; set;
        }
    }
}

