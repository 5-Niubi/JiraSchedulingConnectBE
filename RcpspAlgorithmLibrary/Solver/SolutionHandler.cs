using Google.OrTools.Sat;

namespace AlgorithmLibrary.Solver
{
    public class SingleSolutionExtractor
    {
        public ResultModel _solution;
        public ResultModel Solution
        {
            get
            {
                return _solution;
            }
            private set
            {
                _solution = value;
            }
        }

        private CpSolver solver;
        private int numOfTasks;
        private int numOfWorkers;
        private Dictionary<(int, int), BoolVar> A;
        private Dictionary<int, IntVar> taskFinish;
        private Dictionary<int, IntVar> taskBegin;
        private IntVar finishTime;
        private IntVar projectSalary;
        private IntVar projectExper;

        public SingleSolutionExtractor(CpSolver solver, int numOfTasks, int numOfWorkers, Dictionary<(int, int), BoolVar> A, Dictionary<int, IntVar> taskBegin, Dictionary<int, IntVar> taskFinish, IntVar projectExper, IntVar projectSalary, IntVar finishTime)
        {
            this.numOfTasks = numOfTasks;
            this.numOfWorkers = numOfWorkers;
            this.A = A;
            this.taskBegin = taskBegin;
            this.taskFinish = taskFinish;
            this.projectExper = projectExper;
            this.projectSalary = projectSalary;
            this.finishTime = finishTime;
            this.solver = solver;
        }

        public void SolutionBuilder()
        {
            var result = new ResultModel
            {
                Assign = new int[numOfTasks],
                TaskBegin = new int[numOfTasks],
                TaskFinish = new int[numOfTasks],
            };

            for (int t = 0; t < numOfTasks; ++t)
            {
                result.TaskBegin[t] = Convert.ToInt32(solver.Value(taskBegin[t]));
                result.TaskFinish[t] = Convert.ToInt32(solver.Value(taskFinish[t]));
                for (int w = 0; w < numOfWorkers; ++w)
                {
                    if (solver.Value(A[(t, w)]) == 1)
                        result.Assign[t] = w;
                }
            }

            result.TotalExper = Convert.ToInt32(solver.Value(projectExper));
            result.TotalSalary = Convert.ToInt32(solver.Value(projectSalary));
            result.TimeFinish = Convert.ToInt32(solver.Value(finishTime));
            Solution = result;
        }
    }

    public class MultipleSolutionExtractor : CpSolverSolutionCallback
    {
        public MultipleSolutionExtractor(int numOfTasks, int numOfWorkers, BoolVar[,] A, IntVar[] taskBegin, IntVar[] taskFinish, IntVar projectExper, IntVar projectSalary, IntVar finishTime)
        {
            this.numOfTasks = numOfTasks;
            this.numOfWorkers = numOfWorkers;
            this.A = A;
            this.taskBegin = taskBegin;
            this.taskFinish = taskFinish;
            this.projectExper = projectExper;
            this.projectSalary = projectSalary;
            this.finishTime = finishTime;
        }

        public override void OnSolutionCallback()
        {
            {
                // Console.WriteLine(String.Format("Solution #{0}: time = {1:F2} s", solution_count_, WallTime()));
                var result = new ResultModel
                {
                    Assign = new int[numOfTasks],
                    TaskBegin = new int[numOfTasks],
                    TaskFinish = new int[numOfTasks],
                };

                for (int t = 0; t < numOfTasks; ++t)
                {
                    result.TaskBegin[t] = Convert.ToInt32(Value(taskBegin[t]));
                    result.TaskFinish[t] = Convert.ToInt32(Value(taskFinish[t]));
                    for (int w = 0; w < numOfWorkers; ++w)
                    {
                        if (Value(A[t, w]) == 1)
                            result.Assign[t] = w;
                    }
                }

                result.TotalExper = Convert.ToInt32(Value(projectExper));
                result.TotalSalary = Convert.ToInt32(Value(projectSalary));
                result.TimeFinish = Convert.ToInt32(Value(finishTime));

                Solutions.Add(result);
                solution_count_++;
            }
        }

        public int SolutionCount()
        {
            return solution_count_;
        }

        private int solution_count_;
        /// <summary>
        /// <para> Return a <see cref="List{ResultModel}"/> of <see cref="ResultModel"/> class </para>
        /// </summary>
        public List<ResultModel> Solutions { get; } = new List<ResultModel>();
        private int numOfTasks;
        private int numOfWorkers;
        private BoolVar[,] A;
        private IntVar[] taskFinish;
        private IntVar[] taskBegin;
        private IntVar finishTime;
        private IntVar projectSalary;
        private IntVar projectExper;
    }
}
