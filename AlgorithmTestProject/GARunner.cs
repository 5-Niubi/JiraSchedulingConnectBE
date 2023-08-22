using AlgorithmLibrary.GA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AlgorithmTestProject
{
    internal class GARunner
    {
        private const string FilePath =
        @"D:\SourceCodes\Scratch\OrToolsDemo\input.txt";

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
        public int[] durationTime { get; set; } = new int[500];
        public int[,] adjacency { get; set; } = new int[500, 500];
        public double[,] Z { get; set; } = new Double[500, 500];
        public int[,] R { get; set; } = new int[500, 500];
        public int[,] K { get; set; } = new int[500, 500];
        public double[,] U { get; set; } = new double[500, 10000];
        public long[] salaryEachTime { get; set; } = new long[500];
        public int Deadline;
        public int Budget;
        public List<List<int>> manAbleDo = new List<List<int>>();
        public int[,] Exper = new int[505, 505];

        private const int BufferSize = 128;

        public static void Run()
        {
            var p = new GARunner();
            using (var fileStream = File.OpenRead(FilePath))
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize))
            {
                string line;
                string[] arrString;

                // doc so luong tasks
                line = streamReader.ReadLine();
                arrString = line.Split(" ");
                p.numOfTask = Convert.ToInt32(arrString[0]);

                // doc duration cua task
                for (int i = 0; i < p.numOfTask; i++)
                {
                    line = streamReader.ReadLine();
                    arrString = line.Split(" ");
                    p.durationTime[i] = Convert.ToInt32(arrString[0]);
                }

                // doc so luong skill
                line = streamReader.ReadLine();
                arrString = line.Split(" ");
                p.numOfSkill = Convert.ToInt32(arrString[0]);

                // doc so luong candidate
                line = streamReader.ReadLine();
                arrString = line.Split(" ");
                p.numOfPeople = Convert.ToInt32(arrString[0]);

                // doc ma tran precedences
                for (int i = 0; i < p.numOfTask; i++)
                {
                    line = streamReader.ReadLine();
                    arrString = line.Split(" ");
                    for (int j = 0; j < p.numOfTask; j++)
                    {
                        p.adjacency[i, j] = Convert.ToInt32(arrString[j]);
                    }
                }

                // doc ma tran candidate experiences
                for (int i = 0; i < p.numOfPeople; i++)
                {
                    line = streamReader.ReadLine();
                    arrString = line.Split(" ");
                    for (int j = 0; j < p.numOfSkill; j++)
                    {
                        p.K[i, j] = Convert.ToInt32(arrString[j]);
                    }
                }

                // doc ma tran required skill
                for (int i = 0; i < p.numOfTask; i++)
                {
                    line = streamReader.ReadLine();
                    arrString = line.Split(" ");
                    for (int j = 0; j < p.numOfSkill; j++)
                    {
                        p.R[i, j] = Convert.ToInt32(arrString[j]);
                    }
                }
                // doc salary cua candidate
                line = streamReader.ReadLine();
                arrString = line.Split(" ");
                for (int i = 0; i < p.numOfPeople; i++)
                {
                    p.salaryEachTime[i] = Convert.ToInt32(arrString[i]);
                }

                //// ma tran tuong dong
                //for (int i = 0; i < p.numOfTask; i++)
                //{
                //    line = streamReader.ReadLine();
                //    arrString = line.Split(" ");
                //    for (int j = 0; j < p.numOfTask; j++)
                //    {
                //        p.Z[i, j] = Convert.ToDouble(arrString[j]);
                //    }
                //}


                // deadline
                line = streamReader.ReadLine();
                arrString = line.Split(" ");
                p.Deadline = Convert.ToInt32(arrString[0]);

                // doc candidate concentration
                for (int i = 0; i < p.numOfPeople; i++)
                {
                    line = streamReader.ReadLine();
                    arrString = line.Split(" ");
                    for (int j = 0; j < p.Deadline; j++)
                    {
                        p.U[i, j] = Convert.ToDouble(arrString[j]);
                    }
                }

                // doc budget
                line = streamReader.ReadLine();
                arrString = line.Split(" ");
                p.Budget = Convert.ToInt32(arrString[0]);

                GeneticAlgorithm GA = new GeneticAlgorithm();

                p.manAbleDo = GAHelper.SuitableWorker(p.K, p.R, p.numOfTask, p.numOfPeople, p.numOfSkill);
                p.Exper = GAHelper.TaskExperByWorker(p.K, p.R, p.numOfTask, p.numOfPeople, p.numOfSkill);

                Data d = new Data(p.numOfTask, p.numOfSkill, p.numOfPeople, p.durationTime, p.adjacency, p.salaryEachTime, p.Z, p.U, p.Budget, p.Deadline, p.manAbleDo, p.Exper);

                d.Setup();

                // Choose objective time
                d.ChangeWeights(true, false, false);
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
                Chromosome best = population.Chromosomes[0];
                Console.WriteLine(JsonSerializer.Serialize(best, new JsonSerializerOptions { WriteIndented = true }));
                Console.Read();

                int[] task_mem = new int[d.NumOfWorkers + 1];
                for (int j = 0; j < d.NumOfWorkers; j++)
                {
                    task_mem[j] = 0;
                }
                for (int j = 0; j < d.NumOfTasks; j++)
                {
                    int member = best.Genes[j];
                    task_mem[member]++;
                }

            }

        }
    }
}
