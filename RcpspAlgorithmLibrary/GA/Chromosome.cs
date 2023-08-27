namespace AlgorithmLibrary.GA

{
    public class Chromosome
    {
        private bool isFitnessChanged;
        private int[] genes;
        private double fitness;
        private long totalSalary;
        private int totalExper;
        private int timeFinish;
        private int[] taskBegin;
        private int[] taskFinish;
        private int[] workerStart;
        private int[] workerFinish;

        public Chromosome(Data data)
        {
            genes = new int[data.NumOfTasks];
            taskBegin = new int[data.NumOfTasks];
            taskFinish = new int[data.NumOfTasks];
            workerStart = new int[data.NumOfWorkers];
            workerFinish = new int[data.NumOfWorkers];
            fitness = 0;
            totalSalary = 0;
            totalExper = 0;
            timeFinish = 0;
            isFitnessChanged = true;
        }

        public Chromosome InitializeChromosome(Data data)
        {
            Random rand = new();
            for (int t = 0; t < data.NumOfTasks; ++t)
            {
                int taskWorkers = data.SuitableWorkers.ElementAt(t).Count;
                int workerIdx = (int)(rand.NextDouble() * taskWorkers);
                genes[t] = data.SuitableWorkers.ElementAt(t).ElementAt(workerIdx);
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
            List<int> noPredecessors = new();
            int[] lastMan = new int[data.NumOfWorkers];
            int[] timeTask = new int[data.NumOfTasks];
            int[] totalWorkerEffort = new int[data.NumOfWorkers];
            int[] workerTask = new int[data.NumOfTasks];
            for (int w = 0; w < data.NumOfWorkers; ++w)
            {
                workerStart[w] = 0;
                workerFinish[w] = 0;
                lastMan[w] = 0;
                totalWorkerEffort[w] = 0;
            }
            for (int t = 0; t < data.NumOfTasks; ++t)
            {
                timeTask[t] = 0;
                workerTask[t] = genes[t];
            }
            int[] dependencies = new int[data.NumOfTasks];
            for (int t1 = 0; t1 < data.NumOfTasks; ++t1)
            {
                for (int t2 = 0; t2 < data.NumOfTasks; ++t2)
                {
                    if (data.TaskAdjacency[t1, t2] == 1)
                    {
                        dependencies[t2]++;
                    }
                }
            }

            for (int t = 0; t < data.NumOfTasks; ++t)
            {
                if (dependencies[t] == 0)
                    noPredecessors.Add(t);
            }

            while (noPredecessors.Count > 0)
            {
                int np = noPredecessors.ElementAt(0);
                noPredecessors.RemoveAt(0);
                int wt = genes[np];
                totalExper += data.TaskExperByWorker[np, wt];
                int start = 0;
                for (int t = 0; t < data.NumOfTasks; ++t)
                {
                    if (data.TaskAdjacency[t, np] == 1)
                    {
                        start = Math.Max(start, timeTask[t]);
                    }
                }
                start = Math.Max(start, lastMan[wt]);
                if (start == 0)
                    start = 1;
                int end = start;
                double actualEffort = data.TaskDuration[np];
                double similarityAssign = 0;
                //
                //for (int i = 0; i < data.NumOfTasks; ++i)
                //{
                //    if (workerTask[i] == wt)
                //    {
                //        similarityAssign = MathF.Max((float)similarityAssign, (float)data.TaskSimilarity[i, np]);
                //    }
                //}
                //if (similarityAssign > 0.75)
                //    actualEffort *= 0.7;
                //else if (similarityAssign > 0.5)
                //    actualEffort *= 0.8;
                //else if (similarityAssign > 0.25)
                //    actualEffort *= 0.9;
                while (end < data.Deadline)
                {

                    actualEffort -= (data.WorkerEffort[wt, end]);
                    end++;
                    if (actualEffort <= 0)
                        break;
                }
                if (actualEffort > 0)
                {
                    end += (int)(actualEffort + 0.9);
                }
                end--;
                lastMan[wt] = end;
                timeTask[np] = end;
                if (workerStart[wt] == 0)
                    workerStart[wt] = start;
                workerFinish[wt] = Math.Max(workerFinish[wt], end);
                totalWorkerEffort[wt] += (end - start);
                for (int i = 0; i < data.NumOfTasks; ++i)
                {
                    if (data.TaskAdjacency[np, i] == 1)
                    {
                        dependencies[i]--;
                        if (dependencies[i] == 0)
                            noPredecessors.Add(i);
                    }
                }
                taskBegin[np] = start;
                taskFinish[np] = end;
                timeFinish = Math.Max(timeFinish, end);
            }
            for (int w = 0; w < data.NumOfWorkers; ++w)
            {
                totalSalary += data.WorkerSalary[w] * totalWorkerEffort[w];
            }
        }

        public long TotalSalary
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
