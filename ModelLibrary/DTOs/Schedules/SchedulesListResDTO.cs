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
        public int? cost
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
    }
}
