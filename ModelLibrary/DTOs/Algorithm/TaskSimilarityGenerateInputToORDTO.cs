namespace ModelLibrary.DTOs.Algorithm
{
    public class TaskSimilarityGenerateInputToORDTO
    {
        public int TaskCount { get; set; }
        public int SkillCount { get; set; }
        public int FunctionCount { get; set; }
        public int[,] TaskSkillWithLevel { get; set; }
        public int[,] TaskFunctionWithTime { get; set; }
    }
}