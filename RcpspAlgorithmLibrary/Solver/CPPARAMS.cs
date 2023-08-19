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
        public static bool PRESOLVE = true;
        public static int SYMMETRY_LEVEL = 3;
        public static bool FORCE_SYMMETRY = true;
        public static bool LOG_TO_CONSOLE = true;
        public static bool ALL_SOLS = false;
        public static double TIME_LIMIT = 1800;
        public static int LINEARIZATION_LEVEL = 0;
    }
}
