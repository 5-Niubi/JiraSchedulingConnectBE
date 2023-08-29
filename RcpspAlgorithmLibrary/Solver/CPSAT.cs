using AlgorithmLibrary.GA;
using Google.OrTools.Sat;
using Google.OrTools.Util;
using ModelLibrary.DTOs.Algorithm;
using UtilsLibrary.Exceptions;

namespace AlgorithmLibrary.Solver
{
    public class CPSAT
    {
        /// <summary>
        /// Tuuned OR-tools CPSAT for high performance computing
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <exception cref="ModelInvalidException"></exception>
        /// <exception cref="Exception"></exception>
        /// <exception cref="InfeasibleException"></exception>
        public static List<AlgorithmRawOutput> Scheduler(OutputToORDTO data)
        {
            /// Pre-processing
            var expers = GAHelper.TaskExperByWorker(data.WorkerExper, data.TaskExper, data.NumOfTasks, data.NumOfWorkers, data.NumOfSkills);
            var taskWorkerPool = GAHelper.SuitableWorker(data.WorkerExper, data.TaskExper, data.NumOfTasks, data.NumOfWorkers, data.NumOfSkills);
            var allTasks = Enumerable.Range(0, data.NumOfTasks);
            var allWorkers = Enumerable.Range(0, data.NumOfWorkers);
            var allDays = Enumerable.Range(0, data.Deadline);

            /// Unknowns
            var A = new Dictionary<(int, int), BoolVar>();
            var ts = new Dictionary<int, IntVar>();
            var tf = new Dictionary<int, IntVar>();
            var V = new Dictionary<(int, int), BoolVar>();

            // Check for number of tasks
            if (data.NumOfTasks > 100 && data.NumOfWorkers >= 15)
            {
                throw new Exception("Expected a large model, read the description for contact info.");
            }

            /// Pre-configuration
            var maxTime = (data.NumOfTasks >= 100) ? 600 : 250;
            var model = new CpModel();
            var solver = new CpSolver
            {
                StringParameters
                = $"num_workers:{Environment.ProcessorCount};"
                + $"enumerate_all_solutions:{false};"
                + $"log_search_progress:{true};"
                + $"cp_model_presolve:{true};"
                + $"max_time_in_seconds:{maxTime};"
                + $"subsolvers:\"no_lp\";"
                + $"linearization_level:0;"
            };

            /// Holder variable for objective functions
            var maxPTE = data.NumOfSkills * data.NumOfTasks * 5;
            var pte = model.NewIntVar(0, maxPTE, "pte");
            var pteList = new List<IntVar>();
            var pts = model.NewIntVar(0, data.Budget ?? 0 * 10, "pts");
            var ptsList = new List<IntVar>();
            var pft = model.NewIntVar(0, data.Deadline, "pft");

            /// Unknowns Instantiation
            foreach (var i in allTasks)
            {
                var pool = taskWorkerPool.ElementAt(i);
                foreach (var j in pool)
                {
                    A.Add((i, j), model.NewBoolVar($"A[{i}][{j}]"));
                }

                ts[i] = model.NewIntVar(0, data.Deadline, $"ts[{i}]");
                tf[i] = model.NewIntVar(0, data.Deadline, $"tf[{i}]");
                model.Add(tf[i] >= ts[i]);
                model.Add(pft >= tf[i]);

                foreach (var j in allDays)
                {
                    V[(i, j)] = model.NewBoolVar($"V[{i}][{j}]");
                }
            }

            /// Convert to Integer Problem
            var dayEffort = Convert.ToInt32(data.BaseWorkingHour * 10);
            var taskEfforts = new int[data.NumOfTasks];
            foreach (var i in allTasks)
            {
                taskEfforts[i] = Convert.ToInt32(dayEffort * data.TaskDuration[i]);
            }

            var workerEfforts = new int[data.NumOfWorkers, data.Deadline];
            foreach (var i in allWorkers)
            {
                foreach (var j in allDays)
                {
                    workerEfforts[i, j] = Convert.ToInt32(data.WorkerWorkingHours[i, j] * 10);
                }
            }

            /// C01 -> Employees multitasking prohibited
            foreach (var i in allWorkers)
            {
                for (var j = 0; j < data.NumOfTasks - 1; j++)
                {
                    var firstPool = taskWorkerPool.ElementAt(j);
                    for (var k = j + 1; k < data.NumOfTasks; k++)
                    {
                        var secondPool = taskWorkerPool.ElementAt(k);
                        if (firstPool.Contains(i) && secondPool.Contains(i))
                        {
                            var fp = model.NewIntVar(0, data.Deadline, $"fp[{i}][{j}][{k}]");
                            var sp = model.NewIntVar(0, data.Deadline, $"sp[{i}][{j}][{k}]");

                            var ffp = model.NewIntVar(0, data.Deadline, $"ffp[{i}][{j}][{k}]");
                            model.AddMultiplicationEquality(ffp, A[(j, i)], tf[j]);

                            var sfp = model.NewIntVar(0, data.Deadline, $"sfp[{i}][{j}][{k}]");
                            model.AddMultiplicationEquality(sfp, A[(k, i)], tf[k]);

                            model.AddMinEquality(fp, new[] { ffp, sfp });

                            var fsp = model.NewIntVar(0, data.Deadline, $"fsp[{i}][{j}][{k}]");
                            model.AddMultiplicationEquality(fsp, A[(j, i)], ts[j]);

                            var ssp = model.NewIntVar(0, data.Deadline, $"ssp[{i}][{j}][{k}]");
                            model.AddMultiplicationEquality(ssp, A[(k, i)], ts[k]);

                            model.AddMaxEquality(sp, new[] { fsp, ssp });

                            model.Add(fp - sp <= 0).OnlyEnforceIf(new[] { A[(k, i)], A[(j, i)] });
                        }
                    }
                }
            }

            // C02 -> Task precendences contraints
            var dependencyGraph = new Dictionary<int, List<IntVar>>();
            for (int i = 0; i < data.NumOfTasks; i++)
            {
                dependencyGraph[i] = new List<IntVar>();
            }


            for (int i = 0; i < data.NumOfTasks; i++)
            {
                for (int j = 0; j < data.NumOfTasks; j++)
                {
                    if (data.TaskAdjacency[j, i] == 1)
                    {
                        dependencyGraph[i].Add(tf[j]); // vua doi i thanh j
                    }
                }
            }

            for (int i = 0; i < data.NumOfTasks; i++)
            {
                foreach (var j in dependencyGraph[i])
                    model.Add(ts[i] >= j + 1);
            }

            // C03 -> One employee per task
            var ate = new List<BoolVar>();
            var oet = new List<BoolVar>();
            for (int i = 0; i < data.NumOfTasks; i++)
            {
                var employeePool = taskWorkerPool.ElementAt(i);
                foreach (var j in employeePool)
                {
                    ate.Add(A[(i, j)]);
                    oet.Add(A[(i, j)]);
                }
                model.AddExactlyOne(oet);
                oet.Clear();
            }
            model.Add(LinearExpr.Sum(ate) == data.NumOfTasks);

            // SubConstraints --> Day in task
            foreach (var i in allTasks)
            {
                foreach (var j in allDays)
                {
                    var bt = model.NewBoolVar($"lt[{i}, {j}]");

                    model.Add(j <= tf[i]).OnlyEnforceIf(bt);
                    model.Add(j >= ts[i]).OnlyEnforceIf(bt);

                    model.Add(V[(i, j)] == 1).OnlyEnforceIf(bt);
                    model.Add(V[(i, j)] == 0).OnlyEnforceIf(bt.Not());

                }
            }

            foreach (var i in allTasks)
            {
                var employeePool = taskWorkerPool.ElementAt(i);
                foreach (var j in employeePool)
                {
                    /// C04 -> Employee total efforts >= Task efforts
                    var taskEffort = new List<IntVar>();
                    for (int k = 0; k < data.Deadline; k++)
                    {
                        var wde = model.NewIntVar(0, dayEffort, $"wde[{i}][{j}][{k}]");
                        model.AddMultiplicationEquality(wde, new LinearExpr[] { A[(i, j)], V[(i, k)] * workerEfforts[j, k] });
                        taskEffort.Add(wde);
                    }
                    var tmpEffort = model.NewIntVar(0, taskEfforts[i] + dayEffort, $"tmpEffort[{i}][{j}]");
                    //model.Add(tmpEffort == LinearExpr.Sum(taskEffort));

                    //model.Add(tmpEffort >= taskEfforts[i]).OnlyEnforceIf(A[(i, j)]);
                    //model.Add(tmpEffort < taskEfforts[i] + dayEffort).OnlyEnforceIf(A[(i, j)]);

                    model.Add(LinearExpr.Sum(taskEffort) >= taskEfforts[i]).OnlyEnforceIf(A[(i, j)]);
                    model.Add(LinearExpr.Sum(taskEffort) <= taskEfforts[i] + dayEffort).OnlyEnforceIf(A[(i, j)]);

                    /// C05 -> Total scheduling experiences
                    var tmpExp = model.NewIntVar(0, maxPTE, $"tmpExp[{j}][{i}]");
                    model.Add(tmpExp == A[(i, j)] * expers[i, j]);
                    pteList.Add(tmpExp);

                    /// C06 -> Total hiring price
                    var actualDuration = model.NewIntVar(0, data.Deadline, $"ad[{j}][{i}]");
                    model.Add(actualDuration == (tf[i] - ts[i] + 1));
                    var tmpSal = model.NewIntVar(0, data.Budget ?? 0 * 10, $"tmpSal[{j}][{i}]");
                    model.AddMultiplicationEquality(tmpSal, A[(i, j)], actualDuration * data.WorkerSalary[j]);
                    ptsList.Add(tmpSal);
                }
            }
            model.Add(pte == LinearExpr.Sum(pteList));
            model.Add(pts == LinearExpr.Sum(ptsList));

            int w1 = 1;
            int w2 = 1;
            int w3 = 1;

            if (data.ObjectiveSelect[0] == true)
            {
                w1 = 20;
            }
            else if (data.ObjectiveSelect[1] == true)
            {
                w2 = 20;
            }
            else if (data.ObjectiveSelect[2] == true)
            {
                w3 = -20;
            }

            model.Minimize(w1 * pft + w2 * pts + w3 * pte); // linear-weighted sum

            // Running Solver
            var status = solver.Solve(model);

            if (status == CpSolverStatus.ModelInvalid)
            {
                throw new ModelInvalidException("Model is invalid.");
            }
            else if (status == CpSolverStatus.Unknown)
            {
                throw new Exception($"Maximum execution time reached ({maxTime}). No solution found.");
            }
            else if (status == CpSolverStatus.Infeasible)
            {
                throw new InfeasibleException("No solution found for this input.");
            }

            var se = new SingleSolutionExtractor(solver, data.NumOfTasks, data.NumOfWorkers, A, ts, tf, pte, pts, pft);
            se.SolutionBuilder();
            // Dau ra tu day
            var outputList = new List<AlgorithmRawOutput>();

            for (int i = 0; i < 1; i++)
            {
                var output = new AlgorithmRawOutput();
                var individual = se.Solution;
                output.TimeFinish = individual.TimeFinish;
                output.TaskFinish = individual.TaskFinish;
                output.TaskBegin = individual.TaskBegin;
                output.Genes = individual.Assign;
                output.TotalExper = individual.TotalExper;
                output.TotalSalary = individual.TotalSalary;

                outputList.Add(output);
            }
            return outputList;
        }
        public static List<AlgorithmRawOutput> Schedule(OutputToORDTO data)
        {
            Heuristic hr = new Heuristic();
            var z = new double[500, 500];
            var manAbleDo = GAHelper.SuitableWorker(data.WorkerExper, data.TaskExper, data.NumOfTasks, data.NumOfWorkers, data.NumOfSkills);
            var Exper = GAHelper.TaskExperByWorker(data.WorkerExper, data.TaskExper, data.NumOfTasks, data.NumOfWorkers, data.NumOfSkills);

            Data d = new Data(data.NumOfTasks, data.NumOfSkills, data.NumOfWorkers, data.TaskDuration, data.TaskAdjacency, data.WorkerSalary, z, data.WorkerEffort, data.Budget, data.Deadline, manAbleDo, Exper);

            d.Setup();


            if (data.ObjectiveSelect[0] == true)
            {
                d.weight1 = 20;
            }
            else if (data.ObjectiveSelect[1] == true)
            {
                d.weight2 = 20;
            }
            else if (data.ObjectiveSelect[2] == true)
            {
                d.weight3 = 20;
            }


            Population population = new Population(5000).InitializePopulation(d);
            int numOfGen = 0;
            while (numOfGen < GAHelper.NUM_OF_GENARATION)
            {

                population = hr.Evolve(population, d);
                population.SortChromosomesByFitness(d);
                numOfGen++;
            }
            Chromosome best = population.Chromosomes[0];
            // Dau ra tu day
            var outputList = new List<AlgorithmRawOutput>();

            for (int i = 0; i < 1; i++)
            {
                var output = new AlgorithmRawOutput();
                var individual = best;
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

    public class Heuristic
    {
        public Population Evolve(Population population, Data data)
        {
            return MutatePopulation(CrossoverrPopulation(population, data), data);
        }
        private Population CrossoverrPopulation(Population population, Data data)
        {
            Population crossoverPopulation = new Population(population.Chromosomes.Length);
            for (int e = 0; e < GAHelper.NUM_OF_ELITE_CHOMOSOMES; ++e)
            {
                crossoverPopulation.Chromosomes[e] = population.Chromosomes[e];
            }
            for (int e1 = GAHelper.NUM_OF_ELITE_CHOMOSOMES; e1 < population.Chromosomes.Length; ++e1)
            {
                Chromosome chromosome1 = SelectTournamentPopulation(population, data).Chromosomes[0];
                Chromosome chromosome2 = SelectTournamentPopulation(population, data).Chromosomes[0];
                crossoverPopulation.Chromosomes[e1] = CrossoverChromosome(chromosome1, chromosome2, data);
            }
            return crossoverPopulation;
        }
        private Population MutatePopulation(Population population, Data data)
        {
            Population mutatePopulation = new Population(population.Chromosomes.Length);
            for (int e = 0; e < GAHelper.NUM_OF_ELITE_CHOMOSOMES; ++e)
            {
                mutatePopulation.Chromosomes[e] = population.Chromosomes[e];
            }
            for (int e = GAHelper.NUM_OF_ELITE_CHOMOSOMES; e < population.Chromosomes.Length; ++e)
            {

                mutatePopulation.Chromosomes[e] = MutateChromosome(population.Chromosomes[e], data);


            }
            return mutatePopulation;
        }

        private Chromosome CrossoverChromosome(Chromosome chromosome1, Chromosome chromosome2, Data data)
        {
            Random rand = new Random();
            Chromosome crossChromosome = new Chromosome(data);
            for (int e = 0; e < chromosome1.Genes.Length; ++e)
            {
                if (rand.NextDouble() < 0.5) crossChromosome.Genes[e] = chromosome1.Genes[e];
                else crossChromosome.Genes[e] = chromosome2.Genes[e];
            }
            return crossChromosome;
        }

        private Chromosome MutateChromosome(Chromosome chromosome, Data data)
        {
            Random rand = new Random();
            Chromosome mutateChromosome = new Chromosome(data);
            for (int wt = 0; wt < chromosome.Genes.Length; ++wt)
            {
                if (rand.NextDouble() < GAHelper.MUTATION_RATE)
                {
                    int z = data.SuitableWorkers.ElementAt(wt).Count;
                    int c = (int)(rand.NextDouble() * z);
                    mutateChromosome.Genes[wt] = data.SuitableWorkers.ElementAt(wt).ElementAt(c);
                }
                else mutateChromosome.Genes[wt] = chromosome.Genes[wt];
            }
            return mutateChromosome;
        }

        private Population SelectTournamentPopulation(Population population, Data data)
        {
            Random rand = new Random();
            Population tournamentPopulation = new Population(GAHelper.TOURNAMET_SELECTION_SIZE);
            for (int x = 0; x < GAHelper.TOURNAMET_SELECTION_SIZE; ++x)
            {
                int c = (int)(rand.NextDouble() * population.Chromosomes.Length);
                tournamentPopulation.Chromosomes[x] = population.Chromosomes[c];
            }
            tournamentPopulation.SortChromosomesByFitness(data);
            return tournamentPopulation;
        }
    }

}
