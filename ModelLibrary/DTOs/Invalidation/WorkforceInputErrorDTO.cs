namespace ModelLibrary.DTOs.Invalidation
{



    public class WorkingEffortErrorDTO
    {

        public int DayIndex { get; set; } // task id of current task
        public float Effort { get; set; } //  task id it depenedence 
        public String? Message { get; set; }

    }

    public class WorkforceInputErrorDTO
    {

        public int? Id { get; set; }
        public string? AccountId { get; set; }
        public string? Email { get; set; }
        public string? AccountType { get; set; }
        public string? Name { get; set; }
        public string? Avatar { get; set; }
        public string? DisplayName { get; set; }
        public int? Active { get; set; }
        public string? CloudId { get; set; }
        public double? UnitSalary { get; set; }

        public bool? IsDelete { get; set; }

        public string? WorkingType { get; set; }
        public List<SkillRequestErrorDTO>? SkillErrors { get; set; }
        public List<WorkingEffortErrorDTO>? EffortsErrors { get; set; }
        public List<SkillRequestErrorDTO>? newSkills { get; set; }

        public String? Messages { get; set; }

    }

}

