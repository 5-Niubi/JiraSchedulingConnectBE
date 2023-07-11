using System;
namespace ModelLibrary.DTOs.PertSchedule
{
	public class ParameterRequest
	{
		public int ProjectId { get; set; }
        public int Duration { get; set; }
        public float Budget { get; set; }
        public List<int> WorkforceIds { get; set; }
    }
}

