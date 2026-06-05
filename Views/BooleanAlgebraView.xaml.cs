using TipaDiplom.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using TipaDiplom.Models;

namespace TipaDiplom.Views
{
    public partial class BooleanAlgebraView : UserControl
    {
        public BooleanAlgebraView()
        {
            InitializeComponent();
        }

        private (List<string> vars, List<Dictionary<string, bool>> table) GetData()
        {
            var vars = VariablesInput.Text.Split(new[] { ',', ' ' },
                StringSplitOptions.RemoveEmptyEntries).Select(v => v.Trim()).ToList();
            var expression = ExpressionInput.Text;
            var table = BooleanAlgebra.GenerateTruthTable(vars, expression);
            return (vars, table);
        }

        private void TruthTable_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var (vars, table) = GetData();
                var sb = new StringBuilder();

                // Заголовок
                foreach (var v in vars) sb.Append($"{v,5}");
                sb.AppendLine($"{"  F",5}");
                sb.AppendLine(new string('─', (vars.Count + 1) * 5));

                foreach (var row in table)
                {
                    foreach (var v in vars)
                        sb.Append($"{(row[v] ? "1" : "0"),5}");
                    sb.AppendLine($"{(row["Result"] ? "1" : "0"),5}");
                }
                ResultText.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                ResultText.Text = $"Ошибка: {ex.Message}";
            }
        }

        private void SDNF_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var (vars, table) = GetData();
                ResultText.Text = $"СДНФ:\n{BooleanAlgebra.GetSDNF(vars, table)}";
            }
            catch (Exception ex) { ResultText.Text = $"Ошибка: {ex.Message}"; }
        }

        private void SKNF_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var (vars, table) = GetData();
                ResultText.Text = $"СКНФ:\n{BooleanAlgebra.GetSKNF(vars, table)}";
            }
            catch (Exception ex) { ResultText.Text = $"Ошибка: {ex.Message}"; }
        }

        private void Zhegalkin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var (vars, table) = GetData();
                ResultText.Text = $"Полином Жегалкина:\n{BooleanAlgebra.GetZhegalkinPolynomial(vars, table)}";
            }
            catch (Exception ex) { ResultText.Text = $"Ошибка: {ex.Message}"; }
        }

        private void PostProperties_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var (vars, table) = GetData();
                var sb = new StringBuilder("Критерий Поста (свойства функции):\n\n");
                sb.AppendLine($"T₀ (сохраняет 0): {BooleanAlgebra.PreservesZero(table)}");
                sb.AppendLine($"T₁ (сохраняет 1): {BooleanAlgebra.PreservesOne(table)}");
                sb.AppendLine($"S  (самодвойственна): {BooleanAlgebra.IsSelfDual(table)}");
                sb.AppendLine($"M  (монотонна): {BooleanAlgebra.IsMonotone(vars, table)}");
                sb.AppendLine($"L  (линейна): {BooleanAlgebra.IsLinear(vars, table)}");
                ResultText.Text = sb.ToString();
            }
            catch (Exception ex) { ResultText.Text = $"Ошибка: {ex.Message}"; }
        }

        private void FullAnalysis_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var (vars, table) = GetData();
                var sb = new StringBuilder();

                // Таблица истинности
                sb.AppendLine("═══ ТАБЛИЦА ИСТИННОСТИ ═══\n");
                foreach (var v in vars) sb.Append($"{v,5}");
                sb.AppendLine($"{"  F",5}");
                sb.AppendLine(new string('─', (vars.Count + 1) * 5));
                foreach (var row in table)
                {
                    foreach (var v in vars) sb.Append($"{(row[v] ? "1" : "0"),5}");
                    sb.AppendLine($"{(row["Result"] ? "1" : "0"),5}");
                }

                // Вектор функции
                sb.AppendLine($"\nВектор функции: ({string.Join("", table.Select(r => r["Result"] ? "1" : "0"))})");

                // СДНФ и СКНФ
                sb.AppendLine($"\n═══ СДНФ ═══\n{BooleanAlgebra.GetSDNF(vars, table)}");
                sb.AppendLine($"\n═══ СКНФ ═══\n{BooleanAlgebra.GetSKNF(vars, table)}");
                sb.AppendLine($"\n═══ Полином Жегалкина ═══\n{BooleanAlgebra.GetZhegalkinPolynomial(vars, table)}");

                // Свойства
                sb.AppendLine("\n═══ Критерий Поста ═══");
                sb.AppendLine($"T₀ (сохраняет 0): {BooleanAlgebra.PreservesZero(table)}");
                sb.AppendLine($"T₁ (сохраняет 1): {BooleanAlgebra.PreservesOne(table)}");
                sb.AppendLine($"S  (самодвойственна): {BooleanAlgebra.IsSelfDual(table)}");
                sb.AppendLine($"M  (монотонна): {BooleanAlgebra.IsMonotone(vars, table)}");
                sb.AppendLine($"L  (линейна): {BooleanAlgebra.IsLinear(vars, table)}");

                ResultText.Text = sb.ToString();
            }
            catch (Exception ex) { ResultText.Text = $"Ошибка: {ex.Message}"; }
        }
    }
}