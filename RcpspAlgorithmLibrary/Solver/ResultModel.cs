namespace AlgorithmLibrary.Solver
{
        public class ResultModel
        {
            public int TimeFinish { get; set; }
            public int TotalExper { get; set; }
            public int TotalSalary { get; set; }
            public int[] Assign { get; init; } = null!;
            public int[] TaskBegin { get; init; } = null!;
            public int[] TaskFinish { get; set; } = null!;
        }
}