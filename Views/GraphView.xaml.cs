using TipaDiplom.Models;
using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using TipaDiplom.Models;

namespace TipaDiplom.Views
{
    public partial class GraphView : UserControl
    {
        private GraphModel graph;

        public GraphView()
        {
            InitializeComponent();
        }

        private void CreateGraph_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(VertexCountInput.Text, out int n) && n > 0 && n <= 20)
            {
                graph = new GraphModel(n, IsDirectedCheckBox.IsChecked == true);
                ResultText.Text = $"Граф создан: {n} вершин, " +
                    $"{(graph.IsDirected ? "ориентированный" : "неориентированный")}";
                DrawGraph();
            }
            else
            {
                ResultText.Text = "Введите корректное число вершин (1-20)";
            }
        }

        private void AddEdge_Click(object sender, RoutedEventArgs e)
        {
            if (graph == null) { ResultText.Text = "Сначала создайте граф!"; return; }
            var parts = EdgeInput.Text.Split(' ');
            if (parts.Length >= 2 && int.TryParse(parts[0], out int from) &&
                int.TryParse(parts[1], out int to))
            {
                int weight = parts.Length >= 3 && int.TryParse(parts[2], out int w) ? w : 1;
                if (from >= 0 && from < graph.VertexCount && to >= 0 && to < graph.VertexCount)
                {
                    graph.AddEdge(from, to, weight);
                    ResultText.Text = $"Ребро {from} → {to} (вес {weight}) добавлено";
                    DrawGraph();
                }
                else ResultText.Text = "Вершины вне диапазона!";
            }
        }

        private void RemoveEdge_Click(object sender, RoutedEventArgs e)
        {
            if (graph == null) { ResultText.Text = "Сначала создайте граф!"; return; }
            var parts = EdgeInput.Text.Split(' ');
            if (parts.Length >= 2 && int.TryParse(parts[0], out int from) &&
                int.TryParse(parts[1], out int to))
            {
                graph.RemoveEdge(from, to);
                ResultText.Text = $"Ребро {from} → {to} удалено";
                DrawGraph();
            }
        }

        private void DrawGraph()
        {
            GraphCanvas.Children.Clear();
            if (graph == null) return;

            int n = graph.VertexCount;
            double cx = 140, cy = 140, r = 110;
            var positions = new Point[n];

            for (int i = 0; i < n; i++)
            {
                double angle = 2 * Math.PI * i / n - Math.PI / 2;
                positions[i] = new Point(cx + r * Math.Cos(angle), cy + r * Math.Sin(angle));
            }

            // Рёбра
            for (int i = 0; i < n; i++)
            {
                for (int j = (graph.IsDirected ? 0 : i); j < n; j++)
                {
                    if (graph.AdjacencyMatrix[i, j] != 0)
                    {
                        var line = new Line
                        {
                            X1 = positions[i].X,
                            Y1 = positions[i].Y,
                            X2 = positions[j].X,
                            Y2 = positions[j].Y,
                            Stroke = new SolidColorBrush(Color.FromRgb(137, 180, 250)),
                            StrokeThickness = 2
                        };
                        GraphCanvas.Children.Add(line);

                        // Вес
                        if (graph.AdjacencyMatrix[i, j] != 1)
                        {
                            var label = new TextBlock
                            {
                                Text = graph.AdjacencyMatrix[i, j].ToString(),
                                Foreground = new SolidColorBrush(Color.FromRgb(249, 226, 175)),
                                FontSize = 11
                            };
                            Canvas.SetLeft(label, (positions[i].X + positions[j].X) / 2);
                            Canvas.SetTop(label, (positions[i].Y + positions[j].Y) / 2 - 10);
                            GraphCanvas.Children.Add(label);
                        }
                    }
                }
            }

            // Вершины
            for (int i = 0; i < n; i++)
            {
                var ellipse = new Ellipse
                {
                    Width = 30,
                    Height = 30,
                    Fill = new SolidColorBrush(Color.FromRgb(203, 166, 247)),
                    Stroke = new SolidColorBrush(Color.FromRgb(30, 30, 46)),
                    StrokeThickness = 2
                };
                Canvas.SetLeft(ellipse, positions[i].X - 15);
                Canvas.SetTop(ellipse, positions[i].Y - 15);
                GraphCanvas.Children.Add(ellipse);

                var label = new TextBlock
                {
                    Text = i.ToString(),
                    Foreground = new SolidColorBrush(Color.FromRgb(30, 30, 46)),
                    FontWeight = FontWeights.Bold,
                    FontSize = 12
                };
                Canvas.SetLeft(label, positions[i].X - (i >= 10 ? 8 : 4));
                Canvas.SetTop(label, positions[i].Y - 8);
                GraphCanvas.Children.Add(label);
            }
        }

        private void AdjMatrix_Click(object sender, RoutedEventArgs e)
        {
            if (graph == null) return;
            ResultText.Text = "Матрица смежности:\n" +
                              MatrixOperations.MatrixToString(graph.AdjacencyMatrix);
        }

        private void AdjList_Click(object sender, RoutedEventArgs e)
        {
            if (graph == null) return;
            var list = graph.GetAdjacencyList();
            var sb = new StringBuilder("Список смежности:\n");
            foreach (var kvp in list)
                sb.AppendLine($"{kvp.Key}: [{string.Join(", ", kvp.Value)}]");
            ResultText.Text = sb.ToString();
        }

        private void IncMatrix_Click(object sender, RoutedEventArgs e)
        {
            if (graph == null) return;
            ResultText.Text = "Матрица инцидентности:\n" +
                              MatrixOperations.MatrixToString(graph.GetIncidenceMatrix());
        }

        private void Degrees_Click(object sender, RoutedEventArgs e)
        {
            if (graph == null) return;
            var degrees = graph.GetDegrees();
            var sb = new StringBuilder("Степени вершин:\n");
            for (int i = 0; i < degrees.Length; i++)
                sb.AppendLine($"deg({i}) = {degrees[i]}");
            sb.AppendLine($"\nСумма степеней = {degrees.Sum()}");
            ResultText.Text = sb.ToString();
        }

        private void BFS_Click(object sender, RoutedEventArgs e)
        {
            if (graph == null) return;
            var result = graph.BFS(0);
            ResultText.Text = $"BFS (из вершины 0):\n{string.Join(" → ", result)}";
        }

        private void DFS_Click(object sender, RoutedEventArgs e)
        {
            if (graph == null) return;
            var result = graph.DFS(0);
            ResultText.Text = $"DFS (из вершины 0):\n{string.Join(" → ", result)}";
        }

        private void Dijkstra_Click(object sender, RoutedEventArgs e)
        {
            if (graph == null) return;
            var (dist, prev) = graph.Dijkstra(0);
            var sb = new StringBuilder("Алгоритм Дейкстры (из вершины 0):\n\n");
            for (int i = 0; i < dist.Length; i++)
            {
                string d = dist[i] == int.MaxValue ? "∞" : dist[i].ToString();
                sb.AppendLine($"d(0, {i}) = {d}");

                // Восстановление пути
                if (dist[i] != int.MaxValue && i != 0)
                {
                    var path = new System.Collections.Generic.List<int>();
                    int current = i;
                    while (current != -1)
                    {
                        path.Add(current);
                        current = prev[current];
                    }
                    path.Reverse();
                    sb.AppendLine($"  Путь: {string.Join(" → ", path)}");
                }
            }
            ResultText.Text = sb.ToString();
        }

        private void Floyd_Click(object sender, RoutedEventArgs e)
        {
            if (graph == null) return;
            var dist = graph.FloydWarshall();
            var sb = new StringBuilder("Матрица кратчайших расстояний (Флойд-Уоршелл):\n\n");
            for (int i = 0; i < graph.VertexCount; i++)
            {
                for (int j = 0; j < graph.VertexCount; j++)
                {
                    string val = dist[i, j] >= 999999 ? "  ∞" : dist[i, j].ToString().PadLeft(4);
                    sb.Append(val);
                }
                sb.AppendLine();
            }
            ResultText.Text = sb.ToString();
        }

        private void Kruskal_Click(object sender, RoutedEventArgs e)
        {
            if (graph == null) return;
            var mst = graph.Kruskal();
            var sb = new StringBuilder("Минимальное остовное дерево (Краскал):\n\n");
            int totalWeight = 0;
            foreach (var edge in mst)
            {
                sb.AppendLine($"{edge.from} — {edge.to} (вес: {edge.weight})");
                totalWeight += edge.weight;
            }
            sb.AppendLine($"\nОбщий вес: {totalWeight}");
            ResultText.Text = sb.ToString();
        }

        private void Euler_Click(object sender, RoutedEventArgs e)
        {
            if (graph == null) return;
            ResultText.Text = graph.CheckEulerian();
        }

        private void Coloring_Click(object sender, RoutedEventArgs e)
        {
            if (graph == null) return;
            var colors = graph.GreedyColoring();
            var sb = new StringBuilder("Раскраска графа (жадный алгоритм):\n\n");
            string[] colorNames = { "Красный", "Синий", "Зелёный", "Жёлтый",
                                     "Фиолетовый", "Оранжевый", "Розовый", "Голубой" };
            for (int i = 0; i < colors.Length; i++)
            {
                string name = colors[i] < colorNames.Length ? colorNames[colors[i]] : $"Цвет_{colors[i]}";
                sb.AppendLine($"Вершина {i}: {name} (цвет {colors[i]})");
            }
            sb.AppendLine($"\nХроматическое число (оценка сверху): {colors.Max() + 1}");
            ResultText.Text = sb.ToString();
        }

        private void Connectivity_Click(object sender, RoutedEventArgs e)
        {
            if (graph == null) return;
            var sb = new StringBuilder();
            sb.AppendLine($"Граф связный: {graph.IsConnected()}");
            sb.AppendLine($"Количество компонент связности: {graph.GetComponentCount()}");
            ResultText.Text = sb.ToString();
        }

        private void TransitiveClosure_Click(object sender, RoutedEventArgs e)
        {
            if (graph == null) return;
            var closure = MatrixOperations.TransitiveClosure(graph.AdjacencyMatrix);
            ResultText.Text = "Транзитивное замыкание (алгоритм Уоршелла):\n\n" +
                              MatrixOperations.MatrixToString(closure);
        }
    }
}