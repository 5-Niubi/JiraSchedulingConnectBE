using System;
using ModelLibrary.DBModels;
using ModelLibrary.DTOs.Parameters;

namespace ModelLibrary.DTOs.PertSchedule
{
	public class ParameterRequestDTO
	{
		public int ProjectId { get; set; }
        public int Duration { get; set; }
        public float Budget { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? Deadline { get; set; }
        public List<ParameterResourceRequest> ParameterResources { get; set; }
    }
}

