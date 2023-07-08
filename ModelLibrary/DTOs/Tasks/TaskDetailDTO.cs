using ModelLibrary.DTOs.Skills;

namespace ModelLibrary.DTOs.Tasks
{
    public struct TaskDetailDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public double? Duration { get; set; }



        public string? CloudId { get; set; }
        public int? ProjectId { get; set; }
        public int? MilestoneId { get; set; }
        public bool? IsDelete { get; set; }


        public List<int>? PrecedencesId { get; set; }
        public List<int>? RequiredSkillsId { get; set; }
        public List<int>? RequiredSkillsLevel { get; set; }

    }
}

