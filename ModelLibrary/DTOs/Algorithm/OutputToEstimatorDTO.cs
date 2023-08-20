namespace ModelLibrary.DTOs.Algorithm
{
    public class OutputToEstimatorDTO
    {

        public int NumOfTasks
        {
            get; set;
        }
        public int NumOfSkills
        {
            get; set;
        }

        public int[] TaskDuration
        {
            get; set;
        }
        public int[][] TaskAdjacency
        {
            get; set;
        }
        public int[][] TaskExper
        {
            get; set;
        }
        public int[] TaskMilestone
        {
            get; set;
        }


    }
}
