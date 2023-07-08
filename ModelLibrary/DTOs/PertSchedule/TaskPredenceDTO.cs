using System;
namespace ModelLibrary.DTOs.PertSchedule
{
    public class TaskPredenceDTO
    {
        public string Id { get; set; }
        public string? Name { get; set; }
        public double? Duration { get; set; }
        public int? MilestoneId { get; set; }
        public List<int> Precedences { get; set; }
    }
}

