using ModelLibrary.DBModels;

namespace AlgorithmServiceServer.DTOs.AlgorithmController
{
    public class InputToORDTO
    {
        public DateTime StartDate { get; set; }
        public int Deadline { get; set; }
        public int Budget { get; set; }

        public List<ModelLibrary.DBModels.Task> TaskList { get; set; }
        public List<Workforce> WorkerList { get; set; }
        public List<Equipment> EquipmentList { get; set; }
        public List<Skill> SkillList { get; set; }
        public List<Function> FunctionList { get; set; }

        public int? ObjectiveTime, ObjectiveCost, ObjectiveQuality;
    }
}
