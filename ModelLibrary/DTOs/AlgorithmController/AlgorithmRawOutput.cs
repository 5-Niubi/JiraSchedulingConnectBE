using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLibrary.DTOs.AlgorithmController
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
