using ModelLibrary.DBModels;

namespace ModelLibrary.DTOs.Algorithm
{
    public class InputToEstimatorDTO
    {
        public List<DBModels.Task> TaskList
        {
            get; set;
        }
        public List<Skill> SkillList
        {
            get; set;
        }

    }
}
