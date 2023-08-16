using ModelLibrary.DTOs.Algorithm;

namespace AlgorithmLibrary.GA
{
    public class GAExecution
    {
        // --- Param ---
        public int Deadline;
        public int Budget;
        public int numOfTask
        {
            get; set;
        }
        public int numOfPeople
        {
            get; set;
        }
        public int numOfSkill
        {
            get; set;
        }

        // taskDuration
        public int[] durationTime
        {
            get; set;
        }

        // taskAdjacency
        public int[,] adjacency
        {
            get; set;
        }

        // task similarity (Chua dung)
        public double[,] Z
        {
            get; set;
        }

        // task exper (taskSkillWithLevel)
        public int[,] R
        {
            get; set;
        }

        // worker exper (workerSkillWithLevel)
        public int[,] K
        {
            get; set;
        }

        // worker effort (workerEffort)
        public double[,] U
        {
            get; set;
        }

        // workerSalary
        public int[] salaryEachTime
        {
            get; set;
        }
        // ------

        public List<List<int>> manAbleDo = new();
        public int[,] Exper = new int[505, 505];
        public bool[] objectiveChoice = new bool[3];

        public void SetParam(OutputToORDTO param)
        {
            Deadline = param.Deadline;
            Budget = param.Budget;
            numOfTask = param.NumOfTasks;
            numOfPeople = param.NumOfWorkers;
            numOfSkill = param.NumOfSkills;

            durationTime = param.TaskDuration;
            adjacency = param.TaskAdjacency;
            R = param.TaskExper;
            K = param.WorkerExper;
            U = param.WorkerEffort;
            salaryEachTime = param.WorkerSalary;

            objectiveChoice = param.ObjectiveSelect;
        }

        private double[,] GenerateTaskSimilarityMatrix()
        {
            var taskSimilarityMatrix = new double[numOfTask, numOfTask];

            for (var t1 = 0; t1 < numOfTask; t1++)
            {
                for (var t2 = 0; t2 < numOfTask; t2++)
                {
                    // neu 2 task la 1, do tuong dong cua no bang 0
                    if (t1 == t2)
                    {
                        taskSimilarityMatrix[t1, t2] = 0;
                    }
                    // nguoc lai, tinh cosine similarity
                    else
                    {
                        var taskVec1 = new int[numOfSkill];
                        var taskVec2 = new int[numOfSkill];

                        // task skill level la mot tieu chi
                        for (var s = 0; s < numOfSkill; s++)
                        {
                            taskVec1[s] = R[t1, s];
                            taskVec2[s] = R[t2, s];
                        }

                        // tinh toan similarity
                        double dotProduct = 0;
                        double norm1 = 0;
                        double norm2 = 0;

                        for (var element = 0; element < numOfSkill; element++)
                        {
                            dotProduct += taskVec1[element] * taskVec2[element];
                            norm1 += taskVec1[element] * taskVec1[element];
                            norm2 += taskVec2[element] * taskVec2[element];
                        }

                        var cosineSimilarity = dotProduct / (Math.Sqrt(norm1) * Math.Sqrt(norm2));
                        taskSimilarityMatrix[t1, t2] = Math.Round(cosineSimilarity, 1);
                    }
                }
            }
            return taskSimilarityMatrix;
        }

        public List<AlgorithmRawOutput> Run()
        {
            // Calculate task similarity
            Z = GenerateTaskSimilarityMatrix();
            // Bat dau xu ly
            manAbleDo = GAHelper.SuitableWorker(K, R, numOfTask, numOfPeople, numOfSkill);
            Exper = GAHelper.TaskExperByWorker(K, R, numOfTask, numOfPeople, numOfSkill);

            Data d = new(numOfTask, numOfSkill, numOfPeople, durationTime,
                adjacency, salaryEachTime, Z, U, Budget, Deadline, manAbleDo, Exper);
            d.Setup();
            d.ChangeWeights(objectiveChoice[0], objectiveChoice[1], objectiveChoice[2]);
            Population population = new Population(GAHelper.NUM_OF_POPULATION).InitializePopulation(d);
            GeneticAlgorithm geneticAlgorithm = new();
            int numOfGen = 0;
            while (numOfGen < GAHelper.NUM_OF_GENARATION)
            {
                population = geneticAlgorithm.Evolve(population, d);
                population.SortChromosomesByFitness(d);
                numOfGen++;
            }

            // Dau ra tu day
            var outputList = new List<AlgorithmRawOutput>();

            var chromosomeWithDistictFitness = population.Chromosomes
                .GroupBy(c => c.Fitness).Select(c => c.First()).ToList();

            var maxResult = 10;
            if (maxResult > chromosomeWithDistictFitness.Count)
            {
                maxResult = chromosomeWithDistictFitness.Count;
            }

            for (int i = 0; i < maxResult; i++)
            {
                var output = new AlgorithmRawOutput();
                var individual = chromosomeWithDistictFitness[i];
                output.TimeFinish = individual.TimeFinish;
                output.TaskFinish = individual.TaskFinish;
                output.TaskBegin = individual.TaskBegin;
                output.Genes = individual.Genes;
                output.TotalExper = individual.TotalExper;
                output.TotalSalary = individual.TotalSalary;

                outputList.Add(output);
            }

            return outputList;
        }

    }
}

