namespace ModelLibrary.DTOs.Schedules
{
    public class SchedulesListResDTO
    {
        public int? id
        {
            get; set;
        }
        public int? parameterId
        {
            get; set;
        }
        public int? duration
        {
            get; set;
        }
        public long? cost
        {
            get; set;
        }
        public int? quality
        {
            get; set;
        }
        public int? selected
        {
            get; set;
        }
        public DateTime? since
        {
            get; set;
        }
        public string? title
        {
            get; set;
        }
        public string? desciption
        {
            get; set;
        }
        public int? type
        {
            get; set;
        }
        public DateTime? createDatetime
        {
            get; set;
        }
    }
}
