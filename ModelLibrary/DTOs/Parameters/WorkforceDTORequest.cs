using ModelLibrary.DTOs.Skills;

namespace ModelLibrary.DTOs.Parameters
{
    public class WorkforceDTORequest
    {
        public int Id { get; set; }
        public string? AccountId { get; set; }
        public string? Email { get; set; }
        public string? Name { get; set; }
        public string? Avatar { get; set; }
        public string? DisplayName { get; set; }
        public double? UnitSalary { get; set; }


        public int? WorkingType { get; set; }



        public List<float>? WorkingEfforts { get; set; }
        public List<SkillRequestDTO>? WorkforceSkills { get; set; }

    }
}

