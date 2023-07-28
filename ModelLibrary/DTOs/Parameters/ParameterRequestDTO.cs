using ModelLibrary.DTOs.Parameters;

namespace ModelLibrary.DTOs.PertSchedule
{
    public class ParameterRequestDTO
    {
        public int ProjectId { get; set; }
        public int Duration { get; set; }
        public float Budget { get; set; }

        public int? ObjectiveTime { get; set; }
        public int? ObjectiveCost { get; set; }
        public int? ObjectiveQuality { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? Deadline { get; set; }
        public List<ParameterResourceRequest> ParameterResources { get; set; }
    }
}

