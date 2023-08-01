namespace AlgorithmLibrary.GA

{
    public class Data
    {
        public int Budget;
        public int Deadline;
        public int MaxSalary;
        public int MaxDeadline;
        public int MaxExper;
        public int NumOfWorkers;
        public int NumOfTasks;
        public int NumOfSkills;
        public int[] TaskDuration = new int[505];
        public int[,] TaskAdjacency = new int[505, 505];
        public int[] WorkerSalary = new int[505];
        public double[,] TaskSimilarity = new double[505, 505];
        public int[,] TaskExperByWorker = new int[505, 505];
        public double weight1, weight2, weight3;
        public double[,] WorkerEffort = new double[505, 10005];
        public List<List<int>> SuitableWorkers = new List<List<int>>();

        public Data(int numOfTasks, int numOfSkills, int numOfWorkers, int[] TaskDuration, int[,] TaskAdjacency, int[] WorkerSalary, double[,] TaskSimilarity, double[,] WorkerEffort, int Budget, int Deadline, List<List<int>> SuitableWorkers, int[,] TaskExperByWorker)
        {
            this.NumOfTasks = numOfTasks;
            this.NumOfSkills = numOfSkills;
            this.NumOfWorkers = numOfWorkers;
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
            for (int w = 0; w < NumOfWorkers; ++w) MaxSalary = Math.Max(MaxSalary, WorkerSalary[w]);
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
            this.weight1 = 1;
            this.weight2 = 1;
            this.weight3 = 1;
        }

        public void ChangeWeights(bool choice1, bool choice2, bool choice3)
        {
            this.weight1 = 1;
            this.weight2 = 1;
            this.weight3 = 1;

            if (choice1) this.weight1 *= 20;
            if (choice2) this.weight2 *= 20;
            if (choice3) this.weight3 *= 20;
        }

    }
}
