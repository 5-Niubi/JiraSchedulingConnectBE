namespace RcpspAlgorithmLibrary.Models
{
    public class CoverterOutput
    {
        public class ToOR
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

            // Từ các thuộc tính của task, xử lý để ra matrix các task cùng độ tương đồng
            public int[,] TaskSimilarity { get; set; }
            public int[,] TaskExper { get; set; }
            public int[,] TaskFunction { get; set; }
            public int[,] TaskFunctionTime { get; set; }
            public int[,] WorkerExper { get; set; }
            public int[,] WorkerEffort { get; set; }
            public int[] WorkerSalary { get; set; }
            public int[,] EquipmentFunction { get; set; }
            public int[] EquipmentCost
            {
                get; set;
            }

            public class FromOR
            {
                public List<int> workerPerTask = new List<int>();
                public List<int> equipmentPerTask = new List<int>();
                public List<int> taskStartTime = new List<int>();
                public List<int> taskEndTime = new List<int>();
            }
        }
    }
}
