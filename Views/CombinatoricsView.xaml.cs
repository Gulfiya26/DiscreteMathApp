using TipaDiplom.Models;
using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using TipaDiplom.Models;

namespace TipaDiplom.Views
{
    public partial class CombinatoricsView : UserControl
    {
        public CombinatoricsView()
        {
            InitializeComponent();
        }

        private (int n, int k) GetValues()
        {
            int.TryParse(NInput.Text, out int n);
            int.TryParse(KInput.Text, out int k);
            return (n, k);
        }

        private void Factorial_Click(object sender, RoutedEventArgs e)
        {
            var (n, _) = GetValues();
            ResultText.Text = $"{n}! = {Combinatorics.Factorial(n)}";
        }

        private void Permutations_Click(object sender, RoutedEventArgs e)
        {
            var (n, _) = GetValues();
            ResultText.Text = $"P({n}) = {n}! = {Combinatorics.Permutations(n)}";
        }

        private void Arrangements_Click(object sender, RoutedEventArgs e)
        {
            var (n, k) = GetValues();
            ResultText.Text = $"A({n},{k}) = {n}!/({n}-{k})! = {Combinatorics.Arrangements(n, k)}";
        }

        private void Combinations_Click(object sender, RoutedEventArgs e)
        {
            var (n, k) = GetValues();
            ResultText.Text = $"C({n},{k}) = {n}!/({k}!·({n}-{k})!) = {Combinatorics.Combinations(n, k)}";
        }

        private void ArrangementsRep_Click(object sender, RoutedEventArgs e)
        {
            var (n, k) = GetValues();
            ResultText.Text = $"Ā({n},{k}) = {n}^{k} = {Combinatorics.ArrangementsWithRepetition(n, k)}";
        }

        private void CombinationsRep_Click(object sender, RoutedEventArgs e)
        {
            var (n, k) = GetValues();
            ResultText.Text = $"C̄({n},{k}) = C({n}+{k}-1,{k}) = {Combinatorics.CombinationsWithRepetition(n, k)}";
        }

        private void Fibonacci_Click(object sender, RoutedEventArgs e)
        {
            var (n, _) = GetValues();
            var sb = new StringBuilder($"Числа Фибоначчи (F₁ ... F_{n}):\n");
            for (int i = 1; i <= n; i++)
                sb.Append($"F({i})={Combinatorics.Fibonacci(i)}  ");
            ResultText.Text = sb.ToString();
        }

        private void Catalan_Click(object sender, RoutedEventArgs e)
        {
            var (n, _) = GetValues();
            var sb = new StringBuilder($"Числа Каталана (C₀ ... C_{n}):\n");
            for (int i = 0; i <= n; i++)
                sb.Append($"C({i})={Combinatorics.CatalanNumber(i)}  ");
            ResultText.Text = sb.ToString();
        }

        private void Bell_Click(object sender, RoutedEventArgs e)
        {
            var (n, _) = GetValues();
            var sb = new StringBuilder($"Числа Белла (B₀ ... B_{n}):\n");
            for (int i = 0; i <= n; i++)
                sb.Append($"B({i})={Combinatorics.BellNumber(i)}  ");
            ResultText.Text = sb.ToString();
        }

        private void Stirling_Click(object sender, RoutedEventArgs e)
        {
            var (n, k) = GetValues();
            ResultText.Text = $"S({n},{k}) = {Combinatorics.StirlingSecond(n, k)}";
        }

        private void Pascal_Click(object sender, RoutedEventArgs e)
        {
            var (n, _) = GetValues();
            if (n > 15) { ResultText.Text = "Макс. 15 строк"; return; }
            var triangle = Combinatorics.PascalTriangle(n);
            var sb = new StringBuilder($"Треугольник Паскаля ({n} строк):\n\n");
            for (int i = 0; i < n; i++)
            {
                sb.Append(new string(' ', (n - i) * 3));
                for (int j = 0; j <= i; j++)
                    sb.Append($"{triangle[i, j],6}");
                sb.AppendLine();
            }
            ResultText.Text = sb.ToString();
        }

        private void AllPermutations_Click(object sender, RoutedEventArgs e)
        {
            var (n, _) = GetValues();
            if (n > 7) { ResultText.Text = "Макс. n=7 для генерации перестановок"; return; }
            var perms = Combinatorics.GeneratePermutations(n);
            var sb = new StringBuilder($"Все перестановки {n} элементов ({perms.Count} шт.):\n\n");
            int idx = 1;
            foreach (var p in perms)
                sb.AppendLine($"{idx++,3}. ({string.Join(", ", p)})");
            ResultText.Text = sb.ToString();
        }

        private void FullCalc_Click(object sender, RoutedEventArgs e)
        {
            var (n, k) = GetValues();
            var sb = new StringBuilder();
            sb.AppendLine($"n = {n}, k = {k}\n");
            sb.AppendLine($"n! = {Combinatorics.Factorial(n)}");
            sb.AppendLine($"k! = {Combinatorics.Factorial(k)}");
            sb.AppendLine($"P({n}) = {Combinatorics.Permutations(n)}");
            sb.AppendLine($"A({n},{k}) = {Combinatorics.Arrangements(n, k)}");
            sb.AppendLine($"C({n},{k}) = {Combinatorics.Combinations(n, k)}");
            sb.AppendLine($"Ā({n},{k}) = {Combinatorics.ArrangementsWithRepetition(n, k)}");
            sb.AppendLine($"C̄({n},{k}) = {Combinatorics.CombinationsWithRepetition(n, k)}");
            sb.AppendLine($"\nF({n}) = {Combinatorics.Fibonacci(n)}");
            sb.AppendLine($"Каталан C_{n} = {Combinatorics.CatalanNumber(n)}");
            sb.AppendLine($"Белл B_{n} = {Combinatorics.BellNumber(n)}");
            sb.AppendLine($"Стирлинг S({n},{k}) = {Combinatorics.StirlingSecond(n, k)}");
            ResultText.Text = sb.ToString();
        }
    }
}