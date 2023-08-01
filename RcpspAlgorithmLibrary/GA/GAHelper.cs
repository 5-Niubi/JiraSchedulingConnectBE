using UtilsLibrary.Exceptions;

namespace AlgorithmLibrary.GA
{
    public class GAHelper
    {
        public static int NUM_OF_POPULATION = 100;
        public static int NUM_OF_GENARATION = 700;
        public static int NUM_OF_ELITE_CHOMOSOMES = 10;
        public static int TOURNAMET_SELECTION_SIZE = 10;
        public static double MUTATION_RATE = 0.1;

        public static List<List<int>> SuitableWorker(int[,] workerExper, int[,] taskExper, int numOfTasks, int numOfWorkers, int numOfSkills)
        {
            List<List<int>> suitableWorkers = new List<List<int>>();
            for (int t = 0; t < numOfTasks; ++t)
            {
                List<int> taskWorkers = new List<int>();
                for (int w = 0; w < numOfWorkers; ++w)
                {
                    bool ok = true;
                    for (int s = 0; s < numOfSkills; ++s)
                    {
                        if (taskExper[t, s] > workerExper[w, s])
                        {
                            ok = false;
                            break;
                        }
                    }
                    if (ok == true)
                    {
                        taskWorkers.Add(w);
                    }
                }


                if (taskWorkers.Count == 0) throw new NoSuitableWorkerException("No Suitable Worker Was Found!");
                suitableWorkers.Add(taskWorkers);
            }
            return suitableWorkers;
        }

        public static int[,] TaskExperByWorker(int[,] workerExper, int[,] taskExper, int numOfTasks, int numOfWorkers, int numOfSkills)
        {
            int[,] Exper = new int[505, 505];
            for (int t = 0; t < numOfTasks; ++t)
            {
                for (int w = 0; w < numOfWorkers; ++w)
                {
                    bool ok = true;
                    int exper = 0;
                    for (int s = 0; s < numOfSkills; ++s)
                    {
                        if (taskExper[t, s] > workerExper[w, s])
                        {
                            ok = false;
                            break;
                        }
                        else if (taskExper[t, s] > 0)
                        {
                            exper += workerExper[w, s];
                        }
                    }
                    if (ok == true)
                    {
                        Exper[t, w] = exper;
                    }
                }
            }
            return Exper;
        }

    }
}
