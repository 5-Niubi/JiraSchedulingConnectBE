using AlgorithmServiceServer.DTOs.AlgorithmController;
using ModelLibrary.DBModels;
using ModelLibrary.DTOs.Algorithm;
using System.Text.Json;

namespace RcpspAlgorithmLibrary
{
    public class AlgorithmConverter
    {
        public DateTime StartDate { get; private set; }
        public int Deadline { get; private set; }
        public int Budget { get; private set; }

        public int NumOfTasks { get; private set; }
        public int NumOfWorkers { get; private set; }
        public int NumOfSkills { get; private set; }
        public int NumOfEquipments { get; private set; }
        public int NumOfFunctions { get; private set; }

        public List<ModelLibrary.DBModels.Task> TaskList { get; private set; }
        public List<Workforce> WorkerList { get; private set; }
        public List<Equipment> EquipmentList { get; private set; }
        public List<Skill> SkillList { get; private set; }
        public List<Function> FunctionList { get; private set; }

        public AlgorithmConverter(InputToORDTO inputToOR)
        {
            NumOfTasks = inputToOR.TaskList.Count;
            NumOfWorkers = inputToOR.WorkerList.Count;
            NumOfSkills = inputToOR.SkillList.Count;
            NumOfEquipments = inputToOR.EquipmentList.Count;
            NumOfFunctions = inputToOR.FunctionList.Count;

            this.TaskList = inputToOR.TaskList;
            this.WorkerList = inputToOR.WorkerList;
            this.EquipmentList = inputToOR.EquipmentList;
            this.SkillList = inputToOR.SkillList;
            this.FunctionList = inputToOR.FunctionList;
            this.Deadline = inputToOR.Deadline;
        }

        public OutputToORDTO ToOR()
        {
            int[] taskDuration = new int[TaskList.Count];
            int[,] taskAdjacency = new int[TaskList.Count, TaskList.Count]; // Boolean bin matrix
            int[,] workerSkillWithLevel = new int[WorkerList.Count, SkillList.Count]; // Matrix of skill level
            int[,] taskSkillWithLevel = new int[TaskList.Count, SkillList.Count]; // Matrix of skill level
            double[,] workerEffort = new double[WorkerList.Count, Deadline];
            int[] workerSalary = new int[WorkerList.Count];


            // Chua dung
            int[,] taskFunction = new int[TaskList.Count, FunctionList.Count]; // Boolean bin matrix
            int[,] taskFunctionWithTime = new int[TaskList.Count, FunctionList.Count];
            int[,] equipmentFunction = new int[EquipmentList.Count, FunctionList.Count];
            int[] equipmentCost = new int[EquipmentList.Count];
            // ----/ ---

            for (int i = 0; i < TaskList.Count; i++)
            {
                //taskAdjacency[i] = new int[TaskList.Count];
                taskDuration[i] = (int)TaskList[i].Duration;
                for (int j = 0; j < TaskList.Count; j++)
                {
                    taskAdjacency[i, j] = (TaskList[i]
                        .TaskPrecedenceTasks.Where(e => e.TaskId == TaskList[j].Id)
                        .Count() > 0) ? 1 : 0;
                }

                //taskSkillWithLevel[i] = new int[SkillList.Count];
                for (int j = 0; j < SkillList.Count; j++)
                {
                    var skillReq = TaskList[i].TasksSkillsRequireds
                        .Where(e => e.SkillId == SkillList[j].Id).FirstOrDefault();
                    taskSkillWithLevel[i, j] = (int)(skillReq == null ? 0 : skillReq.Level);
                }

                //taskFunction[i] = new int[FunctionList.Count];
                //taskFunctionWithTime[i] = new int[FunctionList.Count];
                for (int j = 0; j < FunctionList.Count; j++)
                {
                    taskFunction[i, j] = TaskList[i].TaskFunctions
                        .Where(tf => tf.FunctionId == FunctionList[j].Id).Count() > 0 ? 1 : 0;
                    var functionRequired = TaskList[i].TaskFunctions
                        .Where(tf => tf.FunctionId == FunctionList[j].Id).FirstOrDefault();
                    taskFunctionWithTime[i, j] =
                        (int)(functionRequired == null ? 0 : functionRequired.RequireTime);
                }
            }

            for (int i = 0; i < WorkerList.Count; i++)
            {

                //workerSkillWithLevel[i] = new int[SkillList.Count];
                for (int j = 0; j < SkillList.Count; j++)
                {
                    var workForceSkill = WorkerList[i].WorkforceSkills
                        .Where(e => e.SkillId == SkillList[j].Id).FirstOrDefault();
                    workerSkillWithLevel[i, j] = (int)(workForceSkill == null ? 0 : workForceSkill.Level);
                }
                double[] workingEffort =
                    JsonSerializer.Deserialize<double[]>(WorkerList[i].WorkingEffort);
                //workerEffort[i] = new double[Deadline];

                int k = 0;
                for (int j = 0; j < Deadline; j++)
                {
                    workerEffort[i, j] = workingEffort[k++];
                    // reset k
                    if (k >= workingEffort.Length)
                    {
                        k = 0;
                    }
                }
                workerSalary[i] = (int)WorkerList[i].UnitSalary;
            }

            for (int i = 0; i < EquipmentList.Count; i++)
            {
                //equipmentFunction[i] = new int[FunctionList.Count];
                for (int j = 0; j < FunctionList.Count; j++)
                {
                    equipmentFunction[i, j] = EquipmentList[i].EquipmentsFunctions
                        .Where(f => f.FunctionId == FunctionList[j].Id).Count() > 0 ? 1 : 0;
                }
                equipmentCost[i] = (int)EquipmentList[i].UnitPrice;
            }

            // Chua dung
            var taskSimilarityGenerateInput = new TaskSimilarityGenerateInputToORDTO();
            taskSimilarityGenerateInput.TaskCount = TaskList.Count;
            taskSimilarityGenerateInput.SkillCount = SkillList.Count;
            taskSimilarityGenerateInput.FunctionCount = FunctionList.Count;
            taskSimilarityGenerateInput.TaskSkillWithLevel = taskSkillWithLevel;
            taskSimilarityGenerateInput.TaskFunctionWithTime = taskFunctionWithTime;
            // -----/ ----

            var output = new OutputToORDTO();
            output.Deadline = Deadline;
            output.Budget = Budget;
            output.NumOfTasks = NumOfTasks;
            output.NumOfWorkers = NumOfWorkers;
            output.NumOfSkills = NumOfSkills;
            output.NumOfEquipments = NumOfEquipments;
            output.NumOfFunctions = NumOfFunctions;
            output.TaskDuration = taskDuration;
            output.TaskAdjacency = taskAdjacency;
            output.TaskExper = taskSkillWithLevel;
            output.TaskFunction = taskFunction;
            output.TaskFunctionTime = taskFunctionWithTime;
            output.WorkerExper = workerSkillWithLevel;
            output.WorkerSalary = workerSalary;
            output.EquipmentFunction = equipmentFunction;
            output.EquipmentCost = equipmentCost;
            output.WorkerEffort = workerEffort;

            // output.taskSimilarityGenerateInput = taskSimilarityGenerateInput;

            return output;
        }

        public OutputFromORDTO FromOR(int[] taskWithWorker, int[] taskWithEquipment, int[] taskStart, int[] taskEnd)
        {
            var outPut = new OutputFromORDTO();
            for (int i = 0; i < taskWithWorker.Length; i++)
            {
                var task = new TaskOutput();
                task.TaskId = TaskList[i].Id;
                task.WorkerId = WorkerList[taskWithWorker[i]].Id;
                task.StartDate = StartDate.AddDays(taskStart[i]);
                task.EndDate = StartDate.AddDays(taskEnd[i]);
                outPut.Tasks.Add(task);
            }
            for (int i = 0; i < taskWithEquipment.Length; i++)
            {
                outPut.Tasks[i % EquipmentList.Count]
                    .EquipmentId.Add(EquipmentList[taskWithEquipment[i]].Id);
            }
            return outPut;
        }
    }
}