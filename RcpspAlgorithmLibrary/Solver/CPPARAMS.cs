using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorithmLibrary.Solver
{
    public static class CPPARAMS
    {
        public static int THREADS = (int)Math.Floor(Environment.ProcessorCount / 1.5);
        public static string PRESOLVE = "true";
        public static string FORCE_SYMMETRY = "true";
        public static string LOG_TO_CONSOLE = "false";
        public static string ALL_SOLS = "false";
        public static int TIME_LIMIT = 300;
    }
}
