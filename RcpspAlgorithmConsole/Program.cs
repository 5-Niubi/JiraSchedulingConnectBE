// See https://aka.ms/new-console-template for more information
using RcpspAlgorithmLibrary;
using Monster;
using Gurobi;



String dataPath = "/Users/pvm/Desktop/algorithmns/tasks_500_skill_3_1688206533";
DataReader reader = new DataReader(dataPath);
// List<List<int>> TaskAdjacency = new List<List<int>>()
//                                                 {
//                                                     new List<int> {0, 0, 0, 0, 0},
//                                                     new List<int> {1, 0, 0, 0, 1},
//                                                     new List<int> {0, 1, 0, 0, 0},
//                                                     new List<int> {0, 1, 0, 0, 0},
//                                                     new List<int> {0, 0, 1, 1, 0}
//                                                 };


List<List<int>> TaskAdjacency = new List<List<int>>();

for (int i = 0; i < reader.TaskAdjacency.GetLength(0); i++)
{
    List<int> rowList = new List<int>();
    for (int j = 0; j < reader.TaskAdjacency.GetLength(1); j++)
    {
        rowList.Add(reader.TaskAdjacency[i, j]);
    }
    TaskAdjacency.Add(rowList);
}



Console.WriteLine("Hello, World!");
Console.WriteLine("Testing library Reference");
Console.WriteLine("-------------------------");

DirectedGraph directedGraph = new DirectedGraph(0);
directedGraph.LoadData(TaskAdjacency);

// Class1 class1 = new Class1();
Console.WriteLine($"Is DAG:  {directedGraph.IsDAG()}");