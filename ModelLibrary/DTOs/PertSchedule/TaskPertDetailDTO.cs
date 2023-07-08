using System;
using ModelLibrary.DTOs.Skills;

namespace ModelLibrary.DTOs.PertSchedule
{
	public class TaskPertDetailDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public double? Duration { get; set; }
        public int? MilestoneId { get; set; }

        public List<SkillDTO>? RequiredSkills { get; set; }

        public DateTime? CreateDatetime { get; set; }
        public DateTime? DeleteDatetime { get; set; }
    }
}

