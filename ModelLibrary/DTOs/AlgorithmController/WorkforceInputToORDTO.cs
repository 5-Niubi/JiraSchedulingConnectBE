namespace ModelLibrary.DTOs.AlgorithmController
{
    public class WorkforceInputToORDTO
    {
        public int Id { get; set; }
        public List<SkillInputToORDTO> Skills { get; set; }
        public int UnitSalary { get; set; }
    }
}
