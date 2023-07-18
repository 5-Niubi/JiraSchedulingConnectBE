using ModelLibrary.DTOs.Skills;

namespace ModelLibrary.DTOs.Parameters
{
    public class WorkforceDTOResponse
    {
        public int Id { get; set; }
        public string? Email { get; set; }
        public string? Name { get; set; }
        public string? Avatar { get; set; }
        public string? DisplayName { get; set; }
        public double? UnitSalary { get; set; }
        public int? WorkingType { get; set; }
        public string? WorkingEffort { get; set; }
        public List<SkillDTOResponse> Skills { get; set; } = null!;
    }
}

