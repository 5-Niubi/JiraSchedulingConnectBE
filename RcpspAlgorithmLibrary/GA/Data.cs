namespace AlgorithmLibrary.GA

{
    public class Data
    {
        public long? Budget;
        public int Deadline;
        public long MaxSalary;
        public int MaxDeadline;
        public int MaxExper;
        public int NumOfWorkers;
        public int NumOfTasks;
        public int NumOfSkills;
        public int[] TaskDuration = new int[505];
        public int[,] TaskAdjacency = new int[505, 505];
        public long[] WorkerSalary = new long[505];
        public double[,] TaskSimilarity = new double[505, 505];
        public int[,] TaskExperByWorker = new int[505, 505];
        public double weight1, weight2, weight3;
        public double[,] WorkerEffort = new double[505, 10005];
        public List<List<int>> SuitableWorkers = new();

        public Data(int numOfTasks, int numOfSkills, int numOfWorkers, int[] TaskDuration, int[,] TaskAdjacency, long[] WorkerSalary, double[,] TaskSimilarity, double[,] WorkerEffort, long? Budget, int Deadline, List<List<int>> SuitableWorkers, int[,] TaskExperByWorker)
        {
            NumOfTasks = numOfTasks;
            NumOfSkills = numOfSkills;
            NumOfWorkers = numOfWorkers;
            this.TaskAdjacency = TaskAdjacency;
            this.Budget = Budget;
            this.Deadline = Deadline;
            this.TaskExperByWorker = TaskExperByWorker;
            this.SuitableWorkers = SuitableWorkers;
            this.WorkerSalary = WorkerSalary;
            this.TaskSimilarity = TaskSimilarity;
            this.WorkerEffort = WorkerEffort;
            this.TaskDuration = TaskDuration;
        }

        public void Setup() // get the limit max of sumsalary , deadline , exper ;
        {
            for (int w = 0; w < NumOfWorkers; ++w)
                MaxSalary = Math.Max(MaxSalary, WorkerSalary[w]);
            for (int t = 0; t < NumOfTasks; ++t)
            {
                MaxDeadline += TaskDuration[t];
            }
            MaxDeadline += Deadline;
            MaxSalary *= MaxDeadline;
            MaxSalary *= NumOfWorkers;
            for (int t = 0; t < NumOfTasks; ++t)
            {
                int taskExper = 0;
                for (int j = 0; j < NumOfWorkers; ++j)
                {
                    taskExper = Math.Max(taskExper, TaskExperByWorker[t, j]);
                }
                MaxExper += taskExper;
            }
            weight1 = 1;
            weight2 = 1;
            weight3 = 1;
        }

        public void ChangeWeights(bool choice1, bool choice2, bool choice3)
        {
            weight1 = 1;
            weight2 = 1;
            weight3 = 1;

            if (choice1)
                weight1 *= 20;
            if (choice2)
                weight2 *= 20;
            if (choice3)
                weight3 *= 20;
        }

    }
}
