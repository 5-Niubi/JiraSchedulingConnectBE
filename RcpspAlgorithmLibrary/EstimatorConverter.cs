using ModelLibrary.DBModels;
using ModelLibrary.DTOs.Algorithm;

namespace AlgorithmLibrary
{
    public class EstimatorConverter
    {

        public int NumOfTasks
        {
            get; private set;
        }
        public int NumOfSkills
        {
            get; private set;
        }

        public List<ModelLibrary.DBModels.Task> TaskList
        {
            get; private set;
        }
        public List<Skill> SkillList
        {
            get; private set;
        }

        public EstimatorConverter(InputToEstimatorDTO InputToEstimator)
        {
            NumOfTasks = InputToEstimator.TaskList.Count;
            NumOfSkills = InputToEstimator.SkillList.Count;

            TaskList = InputToEstimator.TaskList;
            SkillList = InputToEstimator.SkillList;

        }

        public OutputToEstimatorDTO ToEs()
        {
            int[] TaskDuration = new int[TaskList.Count];
            int[][] TaskAdjacency = new int[TaskList.Count][]; // Boolean bin matrix
            int[][] TaskSkillWithLevel = new int[TaskList.Count][]; // Matrix of skill level
            int[] TaskMilestone = new int[TaskList.Count];

            for (int i = 0; i < TaskList.Count; i++)
            {

                TaskAdjacency[i] = new int[TaskList.Count];
                TaskDuration[i] = (int)TaskList[i].Duration;

                for (int j = 0; j < TaskList.Count; j++)
                {
                    if (j != i)
                    {
                        TaskAdjacency[i][j] = (TaskList[i]
                        .TaskPrecedenceTasks.Where(e => e.PrecedenceId == TaskList[j].Id)
                        .Count() > 0) ? 1 : 0;
                    }
                    else
                    {
                        TaskAdjacency[i][j] = 0;
                    }

                }

                TaskSkillWithLevel[i] = new int[SkillList.Count];
                for (int j = 0; j < SkillList.Count; j++)
                {
                    var skillReq = TaskList[i].TasksSkillsRequireds
                        .Where(e => e.SkillId == SkillList[j].Id).FirstOrDefault();
                    TaskSkillWithLevel[i][j] = (int)(skillReq == null ? 0 : skillReq.Level);
                }

                //milestone
                TaskMilestone[i] = (int)TaskList[i].MilestoneId;


            }





            var output = new OutputToEstimatorDTO();

            output.NumOfTasks = NumOfTasks;
            output.NumOfSkills = NumOfSkills;
            output.TaskDuration = TaskDuration;
            output.TaskAdjacency = TaskAdjacency;
            output.TaskExper = TaskSkillWithLevel;
            output.TaskMilestone = TaskMilestone;



            return output;
        }

        public WorkforceWithMilestoneDTO FromEs(int Id, List<int[]> WorkforceWithSkill)
        {
            //SkillOutputFromEstimatorDTO SkillOutput;
            EstimatedResultDTO EstimatedResult = new();

            Dictionary<string, int> uniqueWorkersCount = new();
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
            List<WorkforceOutputFromEsDTO> WorkforceOutputList = new();
            int j = 0;
            foreach (KeyValuePair<string, int> kvp in uniqueWorkersCount)
            {
                string skillVector = kvp.Key;
                int Quantity = kvp.Value;

                List<int> SkillLevelList = skillVector.Split(',')
                              .Select(int.Parse)
                              .ToList();

                // mapping skill index with skill database
                List<SkillOutputFromEstimatorDTO> SkillOutputList = new();
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


            var workforceWithMilestoneDTO = new WorkforceWithMilestoneDTO();
            workforceWithMilestoneDTO.Id = Id;
            workforceWithMilestoneDTO.WorkforceOutputList = WorkforceOutputList;
            return workforceWithMilestoneDTO;
        }
    }
}
