using ModelLibrary.DTOs.Algorithm;

namespace RcpspAlgorithmLibrary.GA
{
    public class GAExecution
    {
        // --- Param ---
        public int Deadline;
        public int Budget;
        public int numOfTask { get; set; }
        public int numOfPeople { get; set; }
        public int numOfSkill { get; set; }

        // taskDuration
        public int[] durationTime { get; set; }

        // taskAdjacency
        public int[,] adjacency { get; set; }

        // task similarity (Chua dung)
        public double[,] Z { get; set; }

        // task exper (taskSkillWithLevel)
        public int[,] R { get; set; }

        // worker exper (workerSkillWithLevel)
        public int[,] K { get; set; }

        // worker effort (workerEffort)
        public double[,] U { get; set; }

        // workerSalary
        public int[] salaryEachTime { get; set; }

        // ------

        public List<List<int>> manAbleDo = new List<List<int>>();
        public int[,] Exper = new int[505, 505];

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
                        for (var s = 0; s < numOfTask; s++)
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
                        taskSimilarityMatrix[t1, t2] = cosineSimilarity; ;
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

            Data d = new Data(numOfTask, numOfSkill, numOfPeople, durationTime,
                adjacency, salaryEachTime, Z, U, Budget, Deadline, manAbleDo, Exper);
            d.Setup();
            d.ChangeWeights(4);
            Population population = new Population(GAHelper.NUM_OF_POPULATION).InitializePopulation(d);
            GeneticAlgorithm geneticAlgorithm = new GeneticAlgorithm();
            int numOfGen = 0;
            while (numOfGen < GAHelper.NUM_OF_GENARATION)
            {
                Console.WriteLine(numOfGen);
                population = geneticAlgorithm.Evolve(population, d);
                population.SortChromosomesByFitness(d);
                numOfGen++;
            }

            // Dau ra tu day
            Chromosome best = population.Chromosomes[0];

            var outputList = new List<AlgorithmRawOutput>();

            for (int i = 0; i < 1; i++)
            {
                var output = new AlgorithmRawOutput();
                var individual = population.Chromosomes[i];
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

