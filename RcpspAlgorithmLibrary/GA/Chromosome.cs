namespace RcpspAlgorithmLibrary.GA
{
    internal class Chromosome
    {
        private bool isFitnessChanged = true;
        private double fitness = 0;
        

        // -- Su dung phan nay ---
        private int[] taskBegin = new int[505];
        private int[] taskFinish = new int[505];
        private int[] genes;

        private int totalSalary = 0; // Tong chi phi toi uu
        private int totalExper = 0; // Tong chat luong du an
        private int timeFinish = 0;
        // -----


        private int[] workerStart = new int[505];
        private int[] workerFinish = new int[505];

        public Chromosome(int len)
        {
            genes = new int[len];
        }

        public Chromosome InitializeChromosome(Data data)
        {
            Random rand = new Random();
            for (int i = 0; i < data.NumOfTasks; ++i)
            {
                int x = data.SuitableWorkers.ElementAt(i).Count;
                int c = (int)(rand.NextDouble() * x);
                genes[i] = data.SuitableWorkers.ElementAt(i).ElementAt(c);
            }
            return this;
        }

        public double GetFitness(Data data)
        {
            if (isFitnessChanged)
            {
                RecalculateFitness(data);
                double newFitness = (data.weight1 * timeFinish / data.MaxDeadline + data.weight2 * totalSalary / data.MaxSalary + data.weight3 * (data.MaxExper - totalExper) / data.MaxExper);
                fitness = newFitness;
                isFitnessChanged = false;
            }
            return fitness;
        }

        public void RecalculateFitness(Data data)
        {
            List<int> noPredecessors = new List<int>();
            int[] lastMan = new int[505];
            int[] timeTask = new int[505];
            int[] totalWorkerEffort = new int[505];
            int[] workerTask = new int[505];
            for (int w = 1; w <= data.NumOfWorkers; ++w)
            {
                workerStart[w] = 0;
                workerFinish[w] = 0;
                lastMan[w] = 0;
                totalWorkerEffort[w] = 0;
            }
            for (int t = 1; t <= data.NumOfTasks; ++t)
            {
                timeTask[t] = 0;
                workerTask[t] = genes[t - 1];
            }
            int[] DEG = new int[505];
            for (int t = 1; t <= data.NumOfTasks; ++t)
            {
                for (int j = 1; j <= data.NumOfTasks; ++j)
                {
                    if (data.TaskAdjacency[t, j] == 1)
                    {
                        DEG[j]++;
                    }
                }
            }
            for (int i = 1; i <= data.NumOfTasks; ++i)
            {
                if (DEG[i] == 0) noPredecessors.Add(i);
            }

            while (noPredecessors.Count > 0)
            {
                int x = noPredecessors.ElementAt(0);
                noPredecessors.RemoveAt(0);
                int y = genes[x - 1];
                totalExper += data.TaskExperByWorker[x, y];
                int start = 0;
                for (int i = 1; i <= data.NumOfTasks; ++i)
                {
                    if (data.TaskAdjacency[i, x] == 1)
                    {
                        start = Math.Max(start, timeTask[i]);
                    }
                }
                start = Math.Max(start, lastMan[y]);
                if (start == 0) start = 1;
                int end = start;
                double cc = data.TaskDuration[x];
                double maxDec = 0;
                for (int i = 1; i < data.NumOfTasks; ++i)
                {
                    if (workerTask[i] == y)
                    {
                        maxDec = Math.Max(maxDec, data.TaskSimilarity[i, x]);
                    }
                }
                if (maxDec > 0.75) cc *= 0.7;
                else if (maxDec > 0.5) cc *= 0.8;
                else if (maxDec > 0.25) cc *= 0.9;
                while (end <= data.Deadline)
                {
                    cc -= (data.WorkerEffort[y, end]);
                    end++;
                    if (cc <= 0) break;
                }
                if (cc > 0)
                {
                    end += (int)(cc + 0.9);
                }
                lastMan[y] = end;
                timeTask[x] = end;
                if (workerStart[y] == 0) workerStart[y] = start;
                workerFinish[y] = Math.Max(workerFinish[y], end);
                totalWorkerEffort[y] += (end - start);
                for (int i = 1; i <= data.NumOfTasks; ++i)
                {
                    if (data.TaskAdjacency[x, i] == 1)
                    {
                        DEG[i]--;
                        if (DEG[i] == 0) noPredecessors.Add(i);
                    }
                }
                taskBegin[x] = start;
                taskFinish[x] = end;
                timeFinish = Math.Max(timeFinish, end);
            }
            for (int w = 1; w <= data.NumOfWorkers; ++w)
            {
                totalSalary += data.WorkerSalary[w] * totalWorkerEffort[w];
            }
        }

        public int TotalSalary
        {
            get => totalSalary;
        }

        public int TotalExper
        {
            get => totalExper;
        }

        public int TimeFinish
        {
            get => timeFinish;
        }

        public double Fitness
        {
            get => fitness;
        }

        public int[] Genes
        {
            get => genes;
        }

        public int[] TaskBegin
        {
            get => taskBegin;
        }

        public int[] TaskFinish
        {
            get => taskFinish;
        }

        public int[] WorkerStart
        {
            get => workerStart;
        }

        public int[] WorkerFinish
        {
            get => workerFinish;
        }
    }
}
