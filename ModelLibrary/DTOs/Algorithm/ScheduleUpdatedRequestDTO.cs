using Newtonsoft.Json;

namespace ModelLibrary.DTOs.Algorithm
{
    public class ScheduleUpdatedRequestDTO
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Desciption { get; set; }
    }
}