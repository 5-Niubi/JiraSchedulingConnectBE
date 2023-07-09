using System;
using ModelLibrary.DTOs.Skills;
using ModelLibrary.DTOs.Tasks;

namespace ModelLibrary.DTOs.PertSchedule
{
	public class TaskPertViewDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public double? Duration { get; set; }
        public int? MilestoneId { get; set; }

        public List<PrecedenceDTO>? Precedences { get; set; }

    }
}

