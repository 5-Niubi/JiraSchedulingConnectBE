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
     
    }

}
