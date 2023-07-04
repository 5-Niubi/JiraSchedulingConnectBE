using ModelLibrary.DBModels;
using ModelLibrary.DTOs.AlgorithmController;

namespace AlgorithmServiceServer.DTOs.AlgorithmController
{
    public class InputToEstimatorDTO { 
        public List<ModelLibrary.DBModels.Task> TaskList { get; set; }
        public List<Skill> SkillList { get; set; }
    
    }
}
