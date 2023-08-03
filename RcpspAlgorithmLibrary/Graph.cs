using ModelLibrary.DTOs.PertSchedule;

namespace RcpspAlgorithmLibrary

{
    public class DirectedGraph
    {
        public int NumberOfNode;
        public int startNode;
        private List<List<int>> graph = new List<List<int>>();

        public DirectedGraph(int startNode)
        {
            this.startNode = startNode;

        }

        public List<List<int>> Graph
        {
            get => graph; set => graph = value;
        }

        public void AddEdge(int u, int v)
        {
            this.Graph[u].Add(v);
        }

        public void LoadData(List<TaskPrecedencesTaskRequestDTO> TaskList)
        {

            int[][] adjacencyMatrix = new int[TaskList.Count][]; // Boolean bin matrix


            for (int i = 0; i < TaskList.Count; i++)
            {

                adjacencyMatrix[i] = new int[TaskList.Count];

                for (int j = 0; j < TaskList.Count; j++)
                {
                    if (j != i)
                    {
                        adjacencyMatrix[i][j] = (TaskList[i]
                        .TaskPrecedences.Where(e => e == TaskList[j].TaskId)
                        .Count() > 0) ? 1 : 0;
                    }
                    else
                    {
                        adjacencyMatrix[i][j] = 0;
                    }

                }
            }

            NumberOfNode = TaskList.Count;


            for (int i = 0; i < NumberOfNode; i++)
            {
                Graph.Add(new List<int>());
            }


            for (int i = 0; i < NumberOfNode; i++)
            {
                for (int j = 0; j < NumberOfNode; j++)
                {
                    if (adjacencyMatrix[j][i] == 1)
                    {
                        this.AddEdge(i, j);
                    }
                };

            }

            // is validate not exited isolate node
            //IsAnyNodeIsolated(adjacencyMatrix);
        }


        private bool IsAnyNodeIsolated(List<List<int>> adjacencyMatrix)
        {
            for (int i = 0; i < adjacencyMatrix.Count; i++)
            {
                var vecAdj = adjacencyMatrix[i];
                var isValid = false;
                foreach (var e in vecAdj)
                {
                    if (e == 1)
                    {
                        isValid = true;
                        break;
                    }

                }

                if (isValid == false)
                {
                    for (int j = 0; j < adjacencyMatrix.Count; j++)
                    {
                        if (adjacencyMatrix[j][i] == 1)
                        {
                            break;
                        }

                    }

                    throw new Exception("In Graph exited node is isolated");
                }



            }
            return true;
        }


        public bool IsCycle(List<bool> visited, List<bool> path, int start)
        {

            if (path[start] == true)
            {
                return true;

            }

            if (visited[start] == true & path[start] == false)
            {
                return false;
            }

            visited[start] = true;
            path[start] = true;

            // check path from start node to end node by DFS
            List<int> subNodes = Graph[start]; //  Subnodes - child nodes of start node
            foreach (int v in subNodes)
            {
                if (IsCycle(visited, path, v) == true)
                {
                    return true;
                }
            }

            path[start] = false;

            return false;

        }

        public bool IsDAG()
        {
            List<bool> visited = new List<bool>(Enumerable.Repeat(false, this.NumberOfNode));
            List<bool> path = new List<bool>(Enumerable.Repeat(false, this.NumberOfNode));



            if (IsCycle(visited, path, this.startNode))
            {
                return false;
            }


            return true;


        }

        public void LoadDataV1(ModelLibrary.DBModels.Task[]? TaskList)
        {
            int[][] adjacencyMatrix = new int[TaskList.Length][]; // Boolean bin matrix


            for (int i = 0; i < TaskList.Length; i++)
            {

                adjacencyMatrix[i] = new int[TaskList.Length];

                for (int j = 0; j < TaskList.Length; j++)
                {
                    if (j != i)
                    {
                        adjacencyMatrix[i][j] = (TaskList[i]
                        .TaskPrecedenceTasks.Where(e => e.PrecedenceId == TaskList[j].Id)
                        .Count() > 0) ? 1 : 0;
                    }
                    else
                    {
                        adjacencyMatrix[i][j] = 0;
                    }

                }
            }

            NumberOfNode = TaskList.Length;


            for (int i = 0; i < NumberOfNode; i++)
            {
                Graph.Add(new List<int>());
            }


            for (int i = 0; i < NumberOfNode; i++)
            {
                for (int j = 0; j < NumberOfNode; j++)
                {
                    if (adjacencyMatrix[j][i] == 1)
                    {
                        this.AddEdge(i, j);
                    }
                };

            }
        }
    }


}
