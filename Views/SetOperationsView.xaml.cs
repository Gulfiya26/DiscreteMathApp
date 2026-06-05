using TipaDiplom.Models;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using TipaDiplom.Models;

namespace TipaDiplom.Views
{
    public partial class SetOperationsView : UserControl
    {
        public SetOperationsView()
        {
            InitializeComponent();
        }

        private void Union_Click(object sender, RoutedEventArgs e)
        {
            var (a, b, _) = GetSets();
            var result = SetOperations.Union(a, b);
            ResultText.Text = $"A ∪ B = {SetOperations.SetToString(result)}\n|A ∪ B| = {result.Count}";
        }

        private void Intersection_Click(object sender, RoutedEventArgs e)
        {
            var (a, b, _) = GetSets();
            var result = SetOperations.Intersection(a, b);
            ResultText.Text = $"A ∩ B = {SetOperations.SetToString(result)}\n|A ∩ B| = {result.Count}";
        }

        private void Difference_Click(object sender, RoutedEventArgs e)
        {
            var (a, b, _) = GetSets();
            var result = SetOperations.Difference(a, b);
            ResultText.Text = $"A \\ B = {SetOperations.SetToString(result)}\n" +
                              $"B \\ A = {SetOperations.SetToString(SetOperations.Difference(b, a))}";
        }

        private void SymDifference_Click(object sender, RoutedEventArgs e)
        {
            var (a, b, _) = GetSets();
            var result = SetOperations.SymmetricDifference(a, b);
            ResultText.Text = $"A △ B = {SetOperations.SetToString(result)}\n|A △ B| = {result.Count}";
        }

        private void Complement_Click(object sender, RoutedEventArgs e)
        {
            var (a, b, u) = GetSets();
            ResultText.Text = $"A' = {SetOperations.SetToString(SetOperations.Complement(a, u))}\n" +
                              $"B' = {SetOperations.SetToString(SetOperations.Complement(b, u))}";
        }

        private void CartesianProduct_Click(object sender, RoutedEventArgs e)
        {
            var (a, b, _) = GetSets();
            var result = SetOperations.CartesianProduct(a, b);
            var sb = new StringBuilder($"A × B (|A × B| = {result.Count}):\n");
            sb.Append("{");
            sb.Append(string.Join(", ", result.Select(p => $"({p.Item1},{p.Item2})")));
            sb.Append("}");
            ResultText.Text = sb.ToString();
        }

        private void PowerSet_Click(object sender, RoutedEventArgs e)
        {
            var (a, _, _) = GetSets();
            if (a.Count > 10)
            {
                ResultText.Text = "Множество слишком большое для вычисления булеана (макс. 10 элементов)";
                return;
            }
            var result = SetOperations.PowerSet(a);
            var sb = new StringBuilder($"P(A) (|P(A)| = {result.Count} = 2^{a.Count}):\n");
            foreach (var subset in result)
                sb.AppendLine(SetOperations.SetToString(subset));
            ResultText.Text = sb.ToString();
        }

        private void IsSubset_Click(object sender, RoutedEventArgs e)
        {
            var (a, b, _) = GetSets();
            ResultText.Text = $"A ⊆ B: {SetOperations.IsSubset(a, b)}\n" +
                              $"A ⊂ B: {SetOperations.IsProperSubset(a, b)}\n" +
                              $"B ⊆ A: {SetOperations.IsSubset(b, a)}\n" +
                              $"B ⊂ A: {SetOperations.IsProperSubset(b, a)}\n" +
                              $"A = B: {a.SetEquals(b)}";
        }

        private void AllOperations_Click(object sender, RoutedEventArgs e)
        {
            var (a, b, u) = GetSets();
            var sb = new StringBuilder();
            sb.AppendLine($"A = {SetOperations.SetToString(a)}, |A| = {a.Count}");
            sb.AppendLine($"B = {SetOperations.SetToString(b)}, |B| = {b.Count}");
            sb.AppendLine($"U = {SetOperations.SetToString(u)}, |U| = {u.Count}");
            sb.AppendLine();
            sb.AppendLine($"A ∪ B = {SetOperations.SetToString(SetOperations.Union(a, b))}");
            sb.AppendLine($"A ∩ B = {SetOperations.SetToString(SetOperations.Intersection(a, b))}");
            sb.AppendLine($"A \\ B = {SetOperations.SetToString(SetOperations.Difference(a, b))}");
            sb.AppendLine($"B \\ A = {SetOperations.SetToString(SetOperations.Difference(b, a))}");
            sb.AppendLine($"A △ B = {SetOperations.SetToString(SetOperations.SymmetricDifference(a, b))}");
            sb.AppendLine($"A' = {SetOperations.SetToString(SetOperations.Complement(a, u))}");
            sb.AppendLine($"B' = {SetOperations.SetToString(SetOperations.Complement(b, u))}");
            sb.AppendLine();
            sb.AppendLine($"A ⊆ B: {SetOperations.IsSubset(a, b)}");
            sb.AppendLine($"B ⊆ A: {SetOperations.IsSubset(b, a)}");
            sb.AppendLine($"A = B: {a.SetEquals(b)}");

            // Проверка законов де Моргана
            var compA = SetOperations.Complement(a, u);
            var compB = SetOperations.Complement(b, u);
            var unionAB = SetOperations.Union(a, b);
            var interAB = SetOperations.Intersection(a, b);
            sb.AppendLine();
            sb.AppendLine("Законы де Моргана:");
            sb.AppendLine($"(A∪B)' = {SetOperations.SetToString(SetOperations.Complement(unionAB, u))}");
            sb.AppendLine($"A'∩B' = {SetOperations.SetToString(SetOperations.Intersection(compA, compB))}");

            ResultText.Text = sb.ToString();
        }

        private (System.Collections.Generic.HashSet<int>, System.Collections.Generic.HashSet<int>,
                 System.Collections.Generic.HashSet<int>) GetSets()
        {
            return (SetOperations.Parse(SetAInput.Text),
                    SetOperations.Parse(SetBInput.Text),
                    SetOperations.Parse(UniversalSetInput.Text));
        }
    }
}