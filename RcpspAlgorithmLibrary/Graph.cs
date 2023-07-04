namespace RcpspEstimator
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

        public List<List<int>> Graph { get => graph; set => graph = value; }

        public void AddEdge(int u, int v)
        {
            this.Graph[u].Add(v);

        }

        public void LoadData(List<List<int>> adjacencyMatrix)
        {
            this.NumberOfNode = adjacencyMatrix.Count;

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


        private bool IsCycle(List<bool> visited, List<bool> path, int start)
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
    }


}
