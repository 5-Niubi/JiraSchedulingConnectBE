using ModelLibrary.DBModels;

namespace RcpspAlgorithmLibrary
{
    internal class AlgorithmConverter
    {
        public DateTime StartDate { get; set; }
        public int DayEffort { get; set; }
        public int Deadline { get; set; }
        public int Budget { get; set; }
        public int NumOfTasks { get; set; }
        public int NumOfWorkers { get; set; }
        public int NumOfSkills { get; set; }
        public int NumOfEquipments { get; set; }
        public int NumOfFunctions { get; set; }

        public List<ModelLibrary.DBModels.Task> TaskList = new();
        public List<Workforce> WorkerList = new();
        public List<Equipment> EquipmentList = new();

        public List<Skill> SkillList = new();
        public List<Function> FunctionList = new();

        public class Input
        {
            public class InputToOR
            {

            }

        }

        public class Output
        {
            public class OutputToOR
            {
                public int Deadline { get; set; }
                public int Budget { get; set; }
                public int NumOfTasks { get; set; }
                public int NumOfWorkers { get; set; }
                public int NumOfSkills { get; set; }
                public int NumOfEquipments { get; set; }
                public int NumOfFunctions { get; set; }

                public int[] TaskDuration { get; set; } = new int[500];
                public int[,] TaskAdjacency { get; set; } = new int[500, 500];

                // Từ các thuộc tính của task, xử lý để ra matrix các task cùng độ tương đồng
                public int[,] TaskSimilarity { get; set; } = new int[500, 500];
                public int[,] TaskExper { get; set; } = new int[500, 500];
                public int[,] TaskFunction { get; set; } = new int[500, 500];
                public int[,] TaskFunctionTime { get; set; } = new int[500, 500];
                public int[,] WorkerExper { get; set; } = new int[500, 500];
                public int[,] WorkerEffort { get; set; } = new int[500, 10000];
                public int[] WorkerSalary { get; set; } = new int[500];
                public int[,] EquipmentFunction { get; set; } = new int[500, 500];
                public int[] EquipmentCost { get; set; } = new int[500];
            }

            public class OutputFromOR
            {
                public List<int> workerPerTask = new List<int>();
                public List<int> equipmentPerTask = new List<int>();
                public List<int> taskStartTime = new List<int>();
                public List<int> taskEndTime = new List<int>();


                /*
                  A[số lượng tasks] : cứ 1 phần từ thì là index của nhân viên
                 X[số lượng tasks * số lượng equipments] : cứ 1 phần tử
                    thì là index của công cụ

                 VD: A[1] = 3 tức là task 2 đang được giao cho nhân viên số 3

                 10 tasks * 2 equipments X[] = X[20]
                 VD: X[1] = 2 tức là task 2 đang sử dụng equipment 3
                     X[13] = 2 tức là task 4 đang sử dụng equipment 3 

                 START[số lượng task] : cứ 1 phần tử là thời điểm bắt đầu của task
                 END[số lượng task] : cứ 1 phần tử là thời điểm kết thúc của task
                 */

            }
        }

        public void ToOR()
        {
            int[] taskDuration = new int[TaskList.Count];
            int[,] taskAdjacency = new int[TaskList.Count, TaskList.Count]; // Boolean bin matrix
            int[,] taskSkillWithLevel = new int[TaskList.Count, SkillList.Count]; // Matrix of skill level
            int[,] taskFunction = new int[TaskList.Count, FunctionList.Count]; // Boolean bin matrix

            for (int i = 0; i < TaskList.Count; i++)
            {
                taskDuration[i] = (int)TaskList[i].Duration;
                for (int j = 0; j < TaskList.Count; j++)
                {
                    taskAdjacency[i, j] = (TaskList[i]
                        .TaskPrecedenceTasks.Where(e => e.PrecedenceId == TaskList[j].Id).Count() > 0) ? 1 : 0;
                }

                for (int j = 0; j < SkillList.Count; j++)
                {
                    taskSkillWithLevel[i, j] = (int)TaskList[i].TasksSkillsRequireds
                        .Where(e => e.Skill.Id == SkillList[j].Id).FirstOrDefault().Level;
                }
                for (int j = 0; j < SkillList.Count; j++)
                {
                    //taskSkillWithLevel[i, j] = (int)TaskList[i].
                    //    .Where(e => e.Skill.Id == SkillList[j].Id).FirstOrDefault().Level;
                }


            }

            return;
        }
        public void FromOR()
        {
            return;
        }
    }
}
