using ModelLibrary.DBModels;

namespace AlgorithmServiceServer.DTOs.AlgorithmController
{
    public class InputToEstimatorDTO
    {
        public List<ModelLibrary.DBModels.Task> TaskList { get; set; }
        public List<Skill> SkillList { get; set; }

    }
}
