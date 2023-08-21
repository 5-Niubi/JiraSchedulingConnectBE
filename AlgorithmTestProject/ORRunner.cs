using AlgorithmLibrary.Solver;
using ModelLibrary.DTOs.Algorithm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorithmTestProject
{
    internal class ORRunner
    {
        public static void Run()
        {
            var dataModel = new OutputToORDTO();
            var path = @"D:\SourceCodes\Scratch\OrToolsDemo\input.txt";
            var buffer = 128;

            using (var fileStream = File.OpenRead(path))
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, buffer))
            {
                string line;
                string[] arrString;
                dataModel.BaseWorkingHour = 5.5f;

                // doc so luong tasks
                line = streamReader.ReadLine();
                arrString = line.Split(" ");
                dataModel.NumOfTasks = Convert.ToInt32(arrString[0]);

                // doc duration cua task
                dataModel.TaskDuration = new int[dataModel.NumOfTasks];
                for (int i = 0; i < dataModel.NumOfTasks; i++)
                {
                    line = streamReader.ReadLine();
                    arrString = line.Split(" ");
                    dataModel.TaskDuration[i] = Convert.ToInt32(arrString[0]);
                }

                // doc so luong skill
                line = streamReader.ReadLine();
                arrString = line.Split(" ");
                dataModel.NumOfSkills = Convert.ToInt32(arrString[0]);

                // doc so luong candidate
                line = streamReader.ReadLine();
                arrString = line.Split(" ");
                dataModel.NumOfWorkers = Convert.ToInt32(arrString[0]);

                // doc ma tran precedences
                dataModel.TaskAdjacency = new int[dataModel.NumOfTasks, dataModel.NumOfTasks];
                for (int i = 0; i < dataModel.NumOfTasks; i++)
                {
                    line = streamReader.ReadLine();
                    arrString = line.Split(" ");
                    for (int j = 0; j < dataModel.NumOfTasks; j++)
                    {
                        dataModel.TaskAdjacency[i, j] = Convert.ToInt32(arrString[j]);
                    }
                }

                // doc ma tran candidate experiences
                dataModel.WorkerExper = new int[dataModel.NumOfWorkers, dataModel.NumOfSkills];
                for (int i = 0; i < dataModel.NumOfWorkers; i++)
                {
                    line = streamReader.ReadLine();
                    arrString = line.Split(" ");
                    for (int j = 0; j < dataModel.NumOfSkills; j++)
                    {
                        dataModel.WorkerExper[i, j] = Convert.ToInt32(arrString[j]);
                    }
                }

                // doc ma tran required skill
                dataModel.TaskExper = new int[dataModel.NumOfTasks, dataModel.NumOfSkills];
                for (int i = 0; i < dataModel.NumOfTasks; i++)
                {
                    line = streamReader.ReadLine();
                    arrString = line.Split(" ");
                    for (int j = 0; j < dataModel.NumOfSkills; j++)
                    {
                        dataModel.TaskExper[i, j] = Convert.ToInt32(arrString[j]);
                    }
                }

                // doc salary cua candidate
                dataModel.WorkerSalary = new long[dataModel.NumOfWorkers];
                line = streamReader.ReadLine();
                arrString = line.Split(" ");
                for (int i = 0; i < dataModel.NumOfWorkers; i++)
                {
                    dataModel.WorkerSalary[i] = Convert.ToInt32(arrString[i]);
                }

                // deadline
                line = streamReader.ReadLine();
                arrString = line.Split(" ");
                dataModel.Deadline = Convert.ToInt32(arrString[0]);

                // doc candidate concentration
                dataModel.WorkerWorkingHours = new float[dataModel.NumOfWorkers, dataModel.Deadline];
                for (int i = 0; i < dataModel.NumOfWorkers; i++)
                {
                    line = streamReader.ReadLine();
                    arrString = line.Split(" ");
                    for (int j = 0; j < dataModel.Deadline; j++)
                    {
                        dataModel.WorkerWorkingHours[i, j] = float.Parse(arrString[j]) * 5.5f;
                    }
                }

                // doc budget
                line = streamReader.ReadLine();
                arrString = line.Split(" ");
                dataModel.Budget = Convert.ToInt32(arrString[0]);
            }

            dataModel.ObjectiveSelect = new[] { false, false, false };

            var watch = System.Diagnostics.Stopwatch.StartNew();
            //CPSAT_Revert.NoSpanSchedule(dataModel, new[] { 1, 1, 20 });
            CPSAT.Schedule(dataModel);

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            TimeSpan t = TimeSpan.FromMilliseconds(elapsedMs);
            string answer = string.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms",
                                    t.Hours,
                                    t.Minutes,
                                    t.Seconds,
                                    t.Milliseconds);
            Console.WriteLine(answer);
        }
    }
}
