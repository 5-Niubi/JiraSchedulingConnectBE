using AlgorithmLibrary.GA;
using Google.OrTools.Sat;
using Google.OrTools.Util;
using ModelLibrary.DTOs.Algorithm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilsLibrary.Exceptions;

namespace AlgorithmLibrary.Solver
{
    public class CPSAT
    {
        public static List<AlgorithmRawOutput> Schedule(OutputToORDTO data)
        {
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
                $"num_workers:{CPPARAMS.THREADS};" +
                $"enumerate_all_solutions:{CPPARAMS.ALL_SOLS};" +
                $"log_search_progress:{CPPARAMS.LOG_TO_CONSOLE};" +
                $"cp_model_presolve:{CPPARAMS.PRESOLVE};"
            };


            /// Unknowns Instantiation
            for (int i = 0; i < data.NumOfTasks; i++)
            {
                for (int j = 0; j < data.NumOfWorkers; j++)
                {
                    A.Add((i, j), model.NewBoolVar($"A[{i}][{j}]"));
                }
            }

            for (int i = 0; i < data.NumOfTasks; ++i)
            {
                ts[i] = model.NewIntVar(1, data.Deadline, $"ts[{i}]");
                tf[i] = model.NewIntVar(1, data.Deadline, $"tf[{i}]");

                for (int j = 0; j < data.Deadline; j++)
                {
                    V[(i, j)] = model.NewBoolVar($"V[{i}][{j}]");
                }
            }


            /// Holder variable for objective functions
            var atw = new List<BoolVar>();
            var otw = new List<BoolVar>();
            var pte = model.NewIntVarFromDomain(new Domain(0, data.NumOfSkills * data.NumOfTasks * 5), "pte");
            var pteList = new List<IntVar>();
            var pts = model.NewIntVarFromDomain(new Domain(0, data.Budget * 10), "pts");
            var ptsList = new List<IntVar>();
            var pft = model.NewIntVarFromDomain(new Domain(0, data.Deadline), "pft");


            /// Convert to Integer Problem
            var dayEffort = Convert.ToInt32(data.BaseWorkingHour * 10);
            var taskEfforts = new int[data.NumOfTasks];
            for (int i = 0; i < data.NumOfTasks; i++)
            {
                taskEfforts[i] = Convert.ToInt32(data.BaseWorkingHour * data.TaskDuration[i] * 10);
            }

            var workerEfforts = new int[data.NumOfWorkers, data.Deadline];
            for (int i = 0; i < data.NumOfWorkers; i++)
            {
                for (int j = 0; j < data.Deadline; j++)
                {
                    workerEfforts[i, j] = Convert.ToInt32(data.WorkerWorkingHours[i, j] * 10);
                }
            }

            /// C01 -> 1 employee per task
            for (int i = 0; i < data.NumOfTasks; i++)
            {
                for (int j = 0; j < data.NumOfWorkers; j++)
                {
                    otw.Add(A[(i, j)]);
                    atw.Add(A[(i, j)]);
                }
                model.Add(LinearExpr.Sum(otw) == 1);
                otw.Clear();
            }
            model.Add(LinearExpr.Sum(atw) == data.NumOfTasks);

            /// C02 -> Employee proficiency level
            for (int i = 0; i < data.NumOfTasks; i++)
            {
                for (int j = 0; j < data.NumOfWorkers; j++)
                {
                    for (int k = 0; k < data.NumOfSkills; k++)
                    {
                        model.Add(data.WorkerExper[j, k] - A[(i, j)] * data.TaskExper[i, k] >= 0);
                    }
                }
            }

            /// C03 -> Task adjacency
            for (int i = 0; i < data.NumOfTasks - 1; i++)
            {
                for (int j = i + 1; j < data.NumOfTasks; j++)
                {
                    model.Add(data.TaskAdjacency[i, j] * (ts[j] - tf[i] - 1) >= 0);
                }
            }

            /// C04 -> Employees multitasking prohibited
            for (var i = 0; i < data.NumOfWorkers; i++)
            {
                for (var j = 0; j < data.NumOfTasks - 1; j++)
                {
                    for (var k = j + 1; k < data.NumOfTasks; k++)
                    {
                        var fp = model.NewIntVarFromDomain(new Domain(0, data.Deadline), $"fp[{i}][{j}][{k}]");
                        var sp = model.NewIntVarFromDomain(new Domain(0, data.Deadline), $"sp[{i}][{j}][{k}]");

                        var ffp = model.NewIntVarFromDomain(new Domain(0, data.Deadline), $"ffp[{i}][{j}][{k}]");
                        model.AddMultiplicationEquality(ffp, A[(j, i)], tf[j]);

                        var sfp = model.NewIntVarFromDomain(new Domain(0, data.Deadline), $"sfp[{i}][{j}][{k}]");
                        model.AddMultiplicationEquality(sfp, A[(k, i)], tf[k]);

                        model.AddMinEquality(fp, new[] { ffp, sfp });


                        var fsp = model.NewIntVarFromDomain(new Domain(0, data.Deadline), $"fsp[{i}][{j}][{k}]");
                        model.AddMultiplicationEquality(fsp, A[(j, i)], ts[j]);

                        var ssp = model.NewIntVarFromDomain(new Domain(0, data.Deadline), $"ssp[{i}][{j}][{k}]");
                        model.AddMultiplicationEquality(ssp, A[(k, i)], ts[k]);

                        model.AddMaxEquality(sp, new[] { fsp, ssp });

                        model.Add(fp - sp <= 0);
                    }
                }
            }

            /// C00 -> Day in tasks
            for (int i = 0; i < data.NumOfTasks; i++)
            {
                for (int j = 0; j < data.Deadline; j++)
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

                    model.Add(bt + lt + gt == 1);
                }
            }

            /// C05 -> Employee total efforts >= Task efforts
            for (int i = 0; i < data.NumOfTasks; i++)
            {
                for (int j = 0; j < data.NumOfWorkers; j++)
                {
                    var taskEffort = new List<IntVar>();
                    for (int k = 0; k < data.Deadline; k++)
                    {
                        var wde = model.NewIntVarFromDomain(new Domain(0, dayEffort), "wde[{i}][{j}][{k}]");
                        model.AddMultiplicationEquality(wde, new LinearExpr[] { A[(i, j)], V[(i, k)] * workerEfforts[j, k] });
                        taskEffort.Add(wde);
                    }
                    model.Add(LinearExpr.Sum(taskEffort) >= taskEfforts[i]).OnlyEnforceIf(A[(i, j)]);
                    model.Add(LinearExpr.Sum(taskEffort) < taskEfforts[i] + dayEffort).OnlyEnforceIf(A[(i, j)]);
                }
            }

            /// Estimated scheduling endtime
            model.AddMaxEquality(pft, tf.Values);

            /// Total scheduling experiences
            var expers = GAHelper.TaskExperByWorker(data.WorkerExper, data.TaskExper, data.NumOfTasks, data.NumOfWorkers, data.NumOfSkills);
            for (int i = 0; i < data.NumOfTasks; i++)
            {
                for (int j = 0; j < data.NumOfWorkers; j++)
                {
                    var tmpExp = model.NewIntVarFromDomain(new Domain(0, 5 * data.NumOfSkills), $"tmpExp[{j}][{i}]");
                    model.Add(tmpExp == A[(i, j)] * expers[i, j]);
                    pteList.Add(tmpExp);
                }
            }
            model.Add(pte == LinearExpr.Sum(pteList));

            /// Total hiring price
            for (var i = 0; i < data.NumOfTasks; i++)
            {
                for (var j = 0; j < data.NumOfWorkers; j++)
                {
                    var tmpSal = model.NewIntVarFromDomain(new Domain(0, data.Budget * 10), $"tmpSal[{j}][{i}]");
                    model.Add(tmpSal == A[(i, j)] * taskEfforts[i] * data.WorkerSalary[j]);
                    ptsList.Add(tmpSal);
                }
            }

            model.Add(pts == LinearExpr.Sum(ptsList));

            /// C006 --> Total hiring price <= Initial budget

            if (data.ObjectiveSelect[0] == true)
            {
                model.Minimize(pft);
            }
            else if (data.ObjectiveSelect[1] == true)
            {
                model.Maximize(pte);
            }
            else if (data.ObjectiveSelect[2] == true)
            {
                model.Minimize(pts);
            }

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
