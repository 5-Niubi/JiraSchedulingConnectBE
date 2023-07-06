namespace RcpspAlgorithmLibrary.GA
{
    internal class GAHelper
    {
        public static int NUM_OF_POPULATION = 110;
        public static int NUM_OF_GENARATION = 500;
        public static int NUM_OF_ELITE_CHOMOSOMES = 10;
        public static int TOURNAMET_SELECTION_SIZE = 10;
        public static double MUTATION_RATE = 0.1;

        public static List<List<int>> SuitableWorker(int[,] workerExper, int[,] taskExper, int numOfTasks, int numOfWorkers, int numOfSkills)
        {
            List<List<int>> suitableWorkers = new List<List<int>>();
            for (int i = 1; i <= numOfTasks; ++i)
            {
                List<int> lst = new List<int>();
                for (int j = 1; j <= numOfWorkers; ++j)
                {
                    bool ok = true;
                    for (int k = 1; k <= numOfSkills; ++k)
                    {
                        if (taskExper[i, k] > workerExper[j, k])
                        {
                            ok = false;
                        }
                    }
                    if (ok == true)
                    {
                        lst.Add(j);
                    }
                }
                suitableWorkers.Add(lst);
            }
            return suitableWorkers;
        }

        public static int[,] TaskExperByWorker(int[,] K, int[,] R, int n, int m, int s)
        {
            int[,] Exper = new int[505, 505];
            for (int i = 1; i <= n; ++i)
            {
                for (int j = 1; j <= m; ++j)
                {
                    bool ok = true;
                    int cur = 0;
                    for (int k = 1; k <= s; ++k)
                    {
                        if (R[i, k] > K[j, k])
                        {
                            ok = false;
                        }
                        else if (R[i, k] > 0)
                        {
                            cur += K[j, k];
                        }
                    }
                    if (ok == true)
                    {
                        Exper[i, j] = cur;
                    }
                }
            }
            return Exper;
        }

    }
}
