using AlgorithmLibrary.GA;
using Google.OrTools.Sat;
using Google.OrTools.Util;
using ModelLibrary.DTOs.Algorithm;
using UtilsLibrary.Exceptions;

namespace AlgorithmLibrary.Solver
{
    public class CPSAT
    {
        public static List<AlgorithmRawOutput> Schedule(OutputToORDTO data)
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

            /// Pre-configuration
            var model = new CpModel();
            var solver = new CpSolver
            {
                StringParameters =
                $"num_workers:{CPPARAMS.THREADS};"
                + $"enumerate_all_solutions:{CPPARAMS.ALL_SOLS};"
                + $"log_search_progress:{CPPARAMS.LOG_TO_CONSOLE};"
                + $"cp_model_presolve:{CPPARAMS.PRESOLVE};"
                + $"max_time_in_seconds:{CPPARAMS.TIME_LIMIT};"
                + $"subsolvers:\"no_lp\";"
                + $"linearization_level:{CPPARAMS.LINEARIZATION_LEVEL}"
            };

            /// Unknowns Instantiation
            foreach (var i in allTasks)
            {
                var pool = taskWorkerPool.ElementAt(i);
                foreach (var j in pool)
                {
                    A.Add((i, j), model.NewBoolVar($"A[{i}][{j}]"));
                }
            }

            foreach (var i in allTasks)
            {
                ts[i] = model.NewIntVar(1, data.Deadline, $"ts[{i}]");
                tf[i] = model.NewIntVar(1, data.Deadline, $"tf[{i}]");

                foreach (var j in allDays)
                {
                    V[(i, j)] = model.NewBoolVar($"V[{i}][{j}]");
                }
            }


            /// Holder variable for objective functions
            var otw = new List<BoolVar>();
            var atw = new List<BoolVar>();
            var pte = model.NewIntVar(0, data.NumOfSkills * data.NumOfTasks * 5, "pte");
            var pteList = new List<IntVar>();
            var pts = model.NewIntVar(0, data.Budget * 10, "pts");
            var ptsList = new List<IntVar>();
            var pft = model.NewIntVar(0, data.Deadline, "pft");


            /// Convert to Integer Problem
            var dayEffort = Convert.ToInt32(data.BaseWorkingHour * 10);
            var taskEfforts = new int[data.NumOfTasks];
            foreach (var i in allTasks)
            {
                taskEfforts[i] = Convert.ToInt32(data.BaseWorkingHour * data.TaskDuration[i] * 10);
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

                            model.Add(fp - sp <= 0);
                        }
                    }
                }
            }

            foreach (var i in allTasks)
            {
                var pool = taskWorkerPool.ElementAt(i);

                foreach (var j in allDays)
                {
                    var gt = model.NewBoolVar($"gt[{i}][{j}]");
                    var lt = model.NewBoolVar($"lt[{i}][{j}]");
                    var bt = model.NewBoolVar($"lt[{i}][{j}]");

                    model.Add(j <= tf[i]).OnlyEnforceIf(bt);
                    model.Add(j >= ts[i]).OnlyEnforceIf(bt);
                    model.Add(j > tf[i]).OnlyEnforceIf(gt);
                    model.Add(j < ts[i]).OnlyEnforceIf(lt);

                    model.Add(V[(i, j)] == 1).OnlyEnforceIf(bt);
                    model.Add(bt == 1).OnlyEnforceIf(V[(i, j)]);

                    model.AddExactlyOne(new[] { bt, lt, gt });
                }

                for (int j = i + 1; j < data.NumOfTasks; j++)
                {
                    model.Add(data.TaskAdjacency[i, j] * (ts[j] - tf[i] - 1) >= 0);
                }

                foreach (var j in pool)
                {
                    /// C02 -> 1 employee per task
                    atw.Add(A[(i, j)]);
                    otw.Add(A[(i, j)]);

                    /// C03 -> Employee total efforts >= Task efforts
                    var taskEffort = new List<IntVar>();
                    for (int k = 0; k < data.Deadline; k++)
                    {
                        var wde = model.NewIntVarFromDomain(new Domain(0, dayEffort), "wde[{i}][{j}][{k}]");
                        model.AddMultiplicationEquality(wde, new LinearExpr[] { A[(i, j)], V[(i, k)] * workerEfforts[j, k] });
                        taskEffort.Add(wde);
                    }
                    model.Add(LinearExpr.Sum(taskEffort) >= taskEfforts[i]).OnlyEnforceIf(A[(i, j)]);
                    model.Add(LinearExpr.Sum(taskEffort) < taskEfforts[i] + dayEffort).OnlyEnforceIf(A[(i, j)]);

                    /// Total scheduling experiences
                    var tmpExp = model.NewIntVarFromDomain(new Domain(0, 5 * data.NumOfSkills), $"tmpExp[{j}][{i}]");
                    model.Add(tmpExp == A[(i, j)] * expers[i, j]);
                    pteList.Add(tmpExp);

                    /// Total hiring price
                    var tmpSal = model.NewIntVarFromDomain(new Domain(0, data.Budget * 10), $"tmpSal[{j}][{i}]");
                    model.Add(tmpSal == A[(i, j)] * taskEfforts[i] * data.WorkerSalary[j]);
                    ptsList.Add(tmpSal);
                }

                model.Add(LinearExpr.Sum(otw) == 1);
                otw.Clear();
            }
            model.Add(pte == LinearExpr.Sum(pteList));
            model.Add(pts == LinearExpr.Sum(ptsList));
            model.AddMaxEquality(pft, tf.Values);

            int w1 = 1;
            int w2 = 0;
            int w3 = 0;

            if (data.ObjectiveSelect[0] == true)
            {

            }
            else if (data.ObjectiveSelect[1] == true)
            {
                w3 = 0;
                w2 = 20;
            }
            else if (data.ObjectiveSelect[2] == true)
            {
                w2 = 0;
                w3 = 20;
            }

            model.Minimize(w1 * pft + w2 * pte + w3 * pts);

            // Running Solver
            var status = solver.Solve(model);

            if (status == CpSolverStatus.ModelInvalid)
            {
                throw new ModelInvalidException("Input Invalid");
            }
            else if (status == CpSolverStatus.Unknown)
            {
                throw new Exception("Algorithm Unknow Error");
            }
            else if (status == CpSolverStatus.Infeasible)
            {
                throw new InfeasibleException("Input Infeasible");
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
