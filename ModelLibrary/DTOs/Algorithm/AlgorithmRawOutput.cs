namespace ModelLibrary.DTOs.Algorithm
{
    public class AlgorithmRawOutput
    {
        public int TotalSalary { get; set; } // Tong chi phi toi uu
        public int TotalExper { get; set; } // Tong chat luong du an
        public int TimeFinish { get; set; }

        public int[]? TaskBegin { get; set; }
        public int[]? TaskFinish { get; set; }
        public int[]? Genes { get; set; }
    }
}
