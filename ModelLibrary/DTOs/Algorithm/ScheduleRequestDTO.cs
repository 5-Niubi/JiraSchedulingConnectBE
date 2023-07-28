using Newtonsoft.Json;

namespace ModelLibrary.DTOs.Algorithm
{
	[JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
	public class ScheduleRequestDTO
	{
		public int? parameterId { get; set; }
		public int? duration { get; set; }
		public int? cost { get; set; }
		public int? quality { get; set; }
		public string? tasks { get; set; }
		public int? selected { get; set; }
	}
}