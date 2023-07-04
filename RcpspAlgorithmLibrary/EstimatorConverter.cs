using AlgorithmServiceServer;
using AlgorithmServiceServer.DTOs.AlgorithmController;
using ModelLibrary.DBModels;
using ModelLibrary.DTOs.AlgorithmController;
using System.Text.Json;

namespace RcpspAlgorithmLibrary
{
    public class EstimatorConverter
    {

        public int NumOfTasks { get; private set; }
        public int NumOfSkills { get; private set; }

        public List<ModelLibrary.DBModels.Task> TaskList { get; private set; }
        public List<Skill> SkillList { get; private set; }

        public EstimatorConverter(InputToEstimatorDTO InputToEstimator)
        {
            NumOfTasks = InputToEstimator.TaskList.Count;
            NumOfSkills = InputToEstimator.SkillList.Count;


            this.TaskList = InputToEstimator.TaskList;
            this.SkillList = InputToEstimator.SkillList;

        }

        public OutputToEstimatorDTO ToEs()
        {
            int[] taskDuration = new int[TaskList.Count];
            int[][] taskAdjacency = new int[TaskList.Count][]; // Boolean bin matrix
            int[][] taskSkillWithLevel = new int[TaskList.Count][]; // Matrix of skill level

            for (int i = 0; i < TaskList.Count; i++)
            {

                taskAdjacency[i] = new int[TaskList.Count];
                taskDuration[i] = (int)TaskList[i].Duration;

                for (int j = 0; j < TaskList.Count; j++)
                {
                    if (j != i)
                    {
                        taskAdjacency[i][j] = (TaskList[i]
                        .TaskPrecedenceTasks.Where(e => e.PrecedenceId == TaskList[j].Id)
                        .Count() > 0) ? 1 : 0;
                    }
                    else
                    {
                        taskAdjacency[i][j] = 0;
                    }

                }

                taskSkillWithLevel[i] = new int[SkillList.Count];
                for (int j = 0; j < SkillList.Count; j++)
                {
                    var skillReq = TaskList[i].TasksSkillsRequireds
                        .Where(e => e.SkillId == SkillList[j].Id).FirstOrDefault();
                    taskSkillWithLevel[i][j] = (int)(skillReq == null ? 0 : skillReq.Level);
                }


            }



            var taskSimilarityGenerateInput = new TaskSimilarityGenerateInputToORDTO();
            taskSimilarityGenerateInput.TaskCount = TaskList.Count;
            taskSimilarityGenerateInput.SkillCount = SkillList.Count;
            taskSimilarityGenerateInput.TaskSkillWithLevel = taskSkillWithLevel;

            var output = new OutputToEstimatorDTO();

            output.NumOfTasks = NumOfTasks;
            output.NumOfSkills = NumOfSkills;
            output.TaskDuration = taskDuration;
            output.TaskAdjacency = taskAdjacency;
            output.TaskExper = taskSkillWithLevel;


            return output;
        }

        public EstimatedResultDTO FromEs(List<int[]> WorkforceWithSkill)
        {



            //SkillOutputFromEstimatorDTO SkillOutput;
            EstimatedResultDTO EstimatedResult = new EstimatedResultDTO();

            Dictionary<string, int> uniqueWorkersCount = new Dictionary<string, int>();
            // Convert WorkforceWithSkill to EstimatedResults
            for (int i = 0; i < WorkforceWithSkill.Count; i++)
            {
                int[] Skills = WorkforceWithSkill[i];
                string skillVector = string.Join(",", Skills.Select(x => x.ToString()));

                if (uniqueWorkersCount.ContainsKey(skillVector))
                {
                    uniqueWorkersCount[skillVector]++;
                }
                else
                {
                    uniqueWorkersCount[skillVector] = 1;
                }
            }



            WorkforceOutputFromEsDTO WorkforceOutput;
            List<WorkforceOutputFromEsDTO> WorkforceOutputList = new List<WorkforceOutputFromEsDTO>();
            int j = 0;
            foreach (KeyValuePair<string, int> kvp in uniqueWorkersCount)
            {
                string skillVector = kvp.Key;
                int Quantity = kvp.Value;

                List<int> SkillLevelList = skillVector.Split(',')
                              .Select(int.Parse)
                              .ToList();

                // mapping skill index with skill database
                List<SkillOutputFromEstimatorDTO> SkillOutputList = new List<SkillOutputFromEstimatorDTO>();
                for (int i = 0; i < SkillLevelList.Count; i++)
                {

                    var skillLevel = SkillLevelList[i];
                    if (skillLevel > 0)
                    {
                        var skillOutput = new SkillOutputFromEstimatorDTO();
                        skillOutput.Id = SkillList[i].Id;
                        skillOutput.Name = SkillList[i].Name;
                        skillOutput.Level = skillLevel;
                        SkillOutputList.Add(skillOutput);
                    }


                }

                WorkforceOutput = new WorkforceOutputFromEsDTO();
                WorkforceOutput.SkillOutputList = SkillOutputList;
                WorkforceOutput.Quantity = Quantity;
                WorkforceOutput.Id = j;

                WorkforceOutputList.Add(WorkforceOutput);
                j += 1;
            }


            var estimatedResultDTO = new EstimatedResultDTO();
            estimatedResultDTO.WorkforceOutputList = WorkforceOutputList;
            return estimatedResultDTO;
        }
    }
}
