namespace ModelLibrary.DTOs.Algorithm
{
    public class TaskInputToORDTO
    {
        public int Id
        {
            get; set;
        }
        public int Duration
        {
            get; set;
        }
        public List<int> PrecedenceTaskId
        {
            get; set;
        }
        public List<SkillInputToORDTO> SkillRequired
        {
            get; set;
        }
        public List<FunctionInputToORDTO> FunctionRequired
        {
            get; set;
        }
    }
}
