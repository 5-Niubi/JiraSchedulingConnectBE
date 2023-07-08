namespace ModelLibrary.DTOs.Algorithm
{
    public class OutputToORDTO
    {
        public int Deadline { get; set; }
        public int Budget { get; set; }

        public int NumOfTasks { get; set; }
        public int NumOfWorkers { get; set; }
        public int NumOfSkills { get; set; }
        public int NumOfEquipments { get; set; }
        public int NumOfFunctions { get; set; }

        public int[] TaskDuration { get; set; }
        public int[,] TaskAdjacency { get; set; }
        public int[,] TaskExper { get; set; }
        public int[,] TaskFunction { get; set; }
        public int[,] TaskFunctionTime { get; set; }
        public int[,] WorkerExper { get; set; }
        public int[] WorkerSalary { get; set; }
        public int[,] EquipmentFunction { get; set; }
        public int[] EquipmentCost { get; set; }
        public double[,] WorkerEffort { get; set; }

        //public List<ModelLibrary.DBModels.Task> TaskList { get; set; }
        //public List<Workforce> WorkerList { get; set; }
        //public List<Equipment> EquipmentList { get; set; }
        //public List<Skill> SkillList { get; set; }
        //public List<Function> FunctionList { get; set; }

        public TaskSimilarityGenerateInputToORDTO taskSimilarityGenerateInput { get; set; }
    }
}