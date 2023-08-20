using ModelLibrary.DTOs.Algorithm.ScheduleResult;

namespace ModelLibrary.DTOs.Algorithm
{
    public class OutputFromORDTO
    {
        public long totalSalary
        {
            get; set;
        } // Tong chi phi toi uu
        public int totalExper
        {
            get; set;
        } // Tong chat luong du an
        public int timeFinish
        {
            get; set;
        }

        public List<TaskScheduleResultDTO>? tasks
        {
            get; set;
        }

        public OutputFromORDTO()
        {
            tasks = new List<TaskScheduleResultDTO>();
        }
    }
}