namespace ModelLibrary.DTOs.AlgorithmController
{
    public class TaskOutput
    {
        public int TaskId { get; set; }
        public int WorkerId { get; set; }
        public List<int>? EquipmentId { get; set; } // Chua dung

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
    public class OutputFromORDTO
    {
        public int TotalSalary { get; set; } // Tong chi phi toi uu
        public int TotalExper { get; set; } // Tong chat luong du an
        public int TimeFinish { get; set; }

        public List<TaskOutput>? Tasks { get; set; }

    }
}