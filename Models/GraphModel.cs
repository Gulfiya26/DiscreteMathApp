using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TipaDiplom.Models
{
    public class GraphModel
    {
        public int VertexCount { get; set; }
        public int[,] AdjacencyMatrix { get; set; }
        public bool IsDirected { get; set; }

        public GraphModel(int vertexCount, bool isDirected = false)
        {
            VertexCount = vertexCount;
            IsDirected = isDirected;
            AdjacencyMatrix = new int[vertexCount, vertexCount];
        }

        public void AddEdge(int from, int to, int weight = 1)
        {
            AdjacencyMatrix[from, to] = weight;
            if (!IsDirected)
                AdjacencyMatrix[to, from] = weight;
        }

        public void RemoveEdge(int from, int to)
        {
            AdjacencyMatrix[from, to] = 0;
            if (!IsDirected)
                AdjacencyMatrix[to, from] = 0;
        }

        // Список смежности
        public Dictionary<int, List<int>> GetAdjacencyList()
        {
            var list = new Dictionary<int, List<int>>();
            for (int i = 0; i < VertexCount; i++)
            {
                list[i] = new List<int>();
                for (int j = 0; j < VertexCount; j++)
                {
                    if (AdjacencyMatrix[i, j] != 0)
                        list[i].Add(j);
                }
            }
            return list;
        }

        // BFS
        public List<int> BFS(int start)
        {
            var visited = new bool[VertexCount];
            var result = new List<int>();
            var queue = new Queue<int>();

            visited[start] = true;
            queue.Enqueue(start);

            while (queue.Count > 0)
            {
                int v = queue.Dequeue();
                result.Add(v);

                for (int i = 0; i < VertexCount; i++)
                {
                    if (AdjacencyMatrix[v, i] != 0 && !visited[i])
                    {
                        visited[i] = true;
                        queue.Enqueue(i);
                    }
                }
            }
            return result;
        }

        // DFS
        public List<int> DFS(int start)
        {
            var visited = new bool[VertexCount];
            var result = new List<int>();
            DFSHelper(start, visited, result);
            return result;
        }

        private void DFSHelper(int v, bool[] visited, List<int> result)
        {
            visited[v] = true;
            result.Add(v);

            for (int i = 0; i < VertexCount; i++)
            {
                if (AdjacencyMatrix[v, i] != 0 && !visited[i])
                    DFSHelper(i, visited, result);
            }
        }

        // Алгоритм Дейкстры
        public (int[] distances, int[] previous) Dijkstra(int start)
        {
            var dist = new int[VertexCount];
            var prev = new int[VertexCount];
            var visited = new bool[VertexCount];

            for (int i = 0; i < VertexCount; i++)
            {
                dist[i] = int.MaxValue;
                prev[i] = -1;
            }
            dist[start] = 0;

            for (int count = 0; count < VertexCount; count++)
            {
                int u = -1;
                for (int i = 0; i < VertexCount; i++)
                {
                    if (!visited[i] && (u == -1 || dist[i] < dist[u]))
                        u = i;
                }

                if (u == -1 || dist[u] == int.MaxValue) break;
                visited[u] = true;

                for (int v = 0; v < VertexCount; v++)
                {
                    if (AdjacencyMatrix[u, v] != 0 && !visited[v])
                    {
                        int newDist = dist[u] + AdjacencyMatrix[u, v];
                        if (newDist < dist[v])
                        {
                            dist[v] = newDist;
                            prev[v] = u;
                        }
                    }
                }
            }
            return (dist, prev);
        }

        // Алгоритм Флойда-Уоршелла
        public int[,] FloydWarshall()
        {
            int[,] dist = new int[VertexCount, VertexCount];
            int INF = 999999;

            for (int i = 0; i < VertexCount; i++)
            {
                for (int j = 0; j < VertexCount; j++)
                {
                    if (i == j) dist[i, j] = 0;
                    else if (AdjacencyMatrix[i, j] != 0) dist[i, j] = AdjacencyMatrix[i, j];
                    else dist[i, j] = INF;
                }
            }

            for (int k = 0; k < VertexCount; k++)
                for (int i = 0; i < VertexCount; i++)
                    for (int j = 0; j < VertexCount; j++)
                        if (dist[i, k] + dist[k, j] < dist[i, j])
                            dist[i, j] = dist[i, k] + dist[k, j];

            return dist;
        }

        // Проверка на Эйлеров путь/цикл
        public string CheckEulerian()
        {
            int oddCount = 0;
            for (int i = 0; i < VertexCount; i++)
            {
                int degree = 0;
                for (int j = 0; j < VertexCount; j++)
                    if (AdjacencyMatrix[i, j] != 0) degree++;
                if (degree % 2 != 0) oddCount++;
            }

            if (oddCount == 0) return "Граф содержит Эйлеров цикл";
            if (oddCount == 2) return "Граф содержит Эйлеров путь";
            return "Граф не содержит Эйлеров путь/цикл";
        }

        // Степени вершин
        public int[] GetDegrees()
        {
            int[] degrees = new int[VertexCount];
            for (int i = 0; i < VertexCount; i++)
                for (int j = 0; j < VertexCount; j++)
                    if (AdjacencyMatrix[i, j] != 0) degrees[i]++;
            return degrees;
        }

        // Матрица инцидентности
        public int[,] GetIncidenceMatrix()
        {
            var edges = new List<(int, int)>();
            for (int i = 0; i < VertexCount; i++)
            {
                int start = IsDirected ? 0 : i;
                for (int j = start; j < VertexCount; j++)
                {
                    if (AdjacencyMatrix[i, j] != 0)
                        edges.Add((i, j));
                }
            }

            int[,] incidence = new int[VertexCount, edges.Count];
            for (int e = 0; e < edges.Count; e++)
            {
                incidence[edges[e].Item1, e] = 1;
                incidence[edges[e].Item2, e] = IsDirected ? -1 : 1;
                if (edges[e].Item1 == edges[e].Item2)
                    incidence[edges[e].Item1, e] = 2; // петля
            }
            return incidence;
        }

        // Алгоритм Краскала (минимальное остовное дерево)
        public List<(int from, int to, int weight)> Kruskal()
        {
            var edges = new List<(int from, int to, int weight)>();
            for (int i = 0; i < VertexCount; i++)
                for (int j = i + 1; j < VertexCount; j++)
                    if (AdjacencyMatrix[i, j] != 0)
                        edges.Add((i, j, AdjacencyMatrix[i, j]));

            edges.Sort((a, b) => a.weight.CompareTo(b.weight));

            int[] parent = Enumerable.Range(0, VertexCount).ToArray();
            int[] rank = new int[VertexCount];

            int Find(int x) => parent[x] == x ? x : parent[x] = Find(parent[x]);
            void Unite(int x, int y)
            {
                x = Find(x); y = Find(y);
                if (rank[x] < rank[y]) (x, y) = (y, x);
                parent[y] = x;
                if (rank[x] == rank[y]) rank[x]++;
            }

            var mst = new List<(int, int, int)>();
            foreach (var edge in edges)
            {
                if (Find(edge.from) != Find(edge.to))
                {
                    mst.Add(edge);
                    Unite(edge.from, edge.to);
                }
            }
            return mst;
        }

        // Хроматическое число (жадный алгоритм)
        public int[] GreedyColoring()
        {
            int[] colors = new int[VertexCount];
            for (int i = 0; i < colors.Length; i++)
                colors[i] = -1;
            colors[0] = 0;

            for (int v = 1; v < VertexCount; v++)
            {
                var usedColors = new HashSet<int>();
                for (int u = 0; u < VertexCount; u++)
                {
                    if (AdjacencyMatrix[v, u] != 0 && colors[u] != -1)
                        usedColors.Add(colors[u]);
                }

                int color = 0;
                while (usedColors.Contains(color)) color++;
                colors[v] = color;
            }
            return colors;
        }

        // Проверка связности
        public bool IsConnected()
        {
            return BFS(0).Count == VertexCount;
        }

        // Количество компонент связности
        public int GetComponentCount()
        {
            var visited = new bool[VertexCount];
            int count = 0;
            for (int i = 0; i < VertexCount; i++)
            {
                if (!visited[i])
                {
                    count++;
                    var bfs = BFS(i);
                    foreach (var v in bfs) visited[v] = true;
                }
            }
            return count;
        }
    }
}