using ModelLibrary.DBModels;
using RcpspAlgorithmLibrary.Models;
using System.Linq;

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

        public AlgorithmConverter(DateTime startDate, int deadLine, int budget,
             List<ModelLibrary.DBModels.Task> taskList,
             List<Workforce> workerList,
             List<Equipment> equipmentList,
             List<Skill> skillList,
             List<Function> functionList)
        {
            NumOfTasks = taskList.Count;
            NumOfWorkers = workerList.Count;
            NumOfSkills = skillList.Count;
            NumOfEquipments = equipmentList.Count;
            NumOfFunctions = functionList.Count;

            this.TaskList = taskList;
            this.WorkerList = workerList;
            this.EquipmentList = equipmentList;
            this.SkillList = skillList;
            this.FunctionList = functionList;
        }

        public CoverterOutput.ToOR ToOR()
        {
            int[] taskDuration = new int[TaskList.Count];
            int[,] taskAdjacency = new int[TaskList.Count, TaskList.Count]; // Boolean bin matrix
            int[,] taskSkillWithLevel = new int[TaskList.Count, SkillList.Count]; // Matrix of skill level
            int[,] taskFunction = new int[TaskList.Count, FunctionList.Count]; // Boolean bin matrix
            int[,] taskFunctionWithTime = new int[TaskList.Count, FunctionList.Count];
            //TODO: Task Similarity

            int[,] workerSkillWithLevel = new int[WorkerList.Count, SkillList.Count]; // Matrix of skill level
            // TODO: workerEffort

            int[] workerSalary = new int[WorkerList.Count];
            int[,] equipmentFunction = new int[EquipmentList.Count, FunctionList.Count];
            int[] equipmentCost = new int[EquipmentList.Count];
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
                for (int j = 0; j < FunctionList.Count; j++)
                {
                    taskFunction[i, j] = TaskList[i].TaskFunctions
                        .Where(tf => tf.FunctionId == FunctionList[j].Id).Count() > 0 ? 1 : 0;
                    taskFunctionWithTime[i, j] = (int)TaskList[i].TaskFunctions
                        .Where(tf => tf.FunctionId == FunctionList[j].Id).FirstOrDefault()?.RequireTime;
                }
            }

            for (int i = 0; i < WorkerList.Count; i++)
            {
                for (int j = 0; j < SkillList.Count; j++)
                {
                    workerSkillWithLevel[i, j] = (int)WorkerList[i].WorkforceSkills
                        .Where(e => e.SkillId == SkillList[j].Id).FirstOrDefault().Level;
                }
                workerSalary[i] = (int)WorkerList[i].UnitSalary;
            }

            for (int i = 0; i < EquipmentList.Count; i++)
            {
                for (int j = 0; j < FunctionList.Count; j++)
                {
                    equipmentFunction[i, j] = EquipmentList[i].Functions
                        .Where(f => f.Id == FunctionList[j].Id).Count() > 0 ? 1 : 0;
                }
                equipmentCost[i] = (int)EquipmentList[i].UnitPrice;
            }

            var output = new CoverterOutput.ToOR();
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

            return output;
        }
        public void FromOR()
        {

            return;
        }
    }
}
