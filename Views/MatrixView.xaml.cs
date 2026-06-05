using TipaDiplom.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TipaDiplom.Models;

namespace TipaDiplom.Views
{
    public partial class MatrixView : UserControl
    {
        public MatrixView()
        {
            InitializeComponent();
        }

        private int[,] ParseMatrix(string text)
        {
            var lines = text.Trim().Split('\n')
                .Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();
            int rows = lines.Length;
            int cols = lines[0].Trim().Split(new[] { ' ', '\t' },
                StringSplitOptions.RemoveEmptyEntries).Length;
            var matrix = new int[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                var values = lines[i].Trim().Split(new[] { ' ', '\t' },
                    StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j < Math.Min(cols, values.Length); j++)
                    int.TryParse(values[j], out matrix[i, j]);
            }
            return matrix;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var a = ParseMatrix(MatrixAInput.Text);
                var b = ParseMatrix(MatrixBInput.Text);
                ResultText.Text = "A + B =\n" + MatrixOperations.MatrixToString(MatrixOperations.Add(a, b));
            }
            catch (Exception ex) { ResultText.Text = $"Ошибка: {ex.Message}"; }
        }

        private void Multiply_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var a = ParseMatrix(MatrixAInput.Text);
                var b = ParseMatrix(MatrixBInput.Text);
                ResultText.Text = "A × B =\n" + MatrixOperations.MatrixToString(MatrixOperations.Multiply(a, b));
            }
            catch (Exception ex) { ResultText.Text = $"Ошибка: {ex.Message}"; }
        }

        private void Transpose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var a = ParseMatrix(MatrixAInput.Text);
                ResultText.Text = "Aᵀ =\n" + MatrixOperations.MatrixToString(MatrixOperations.Transpose(a));
            }
            catch (Exception ex) { ResultText.Text = $"Ошибка: {ex.Message}"; }
        }

        private void Power_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var a = ParseMatrix(MatrixAInput.Text);
                ResultText.Text = "A² =\n" + MatrixOperations.MatrixToString(MatrixOperations.Power(a, 2));
            }
            catch (Exception ex) { ResultText.Text = $"Ошибка: {ex.Message}"; }
        }

        private void Determinant_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var a = ParseMatrix(MatrixAInput.Text);
                ResultText.Text = $"det(A) = {MatrixOperations.Determinant(a)}";
            }
            catch (Exception ex) { ResultText.Text = $"Ошибка: {ex.Message}"; }
        }

        private void BooleanAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var a = ParseMatrix(MatrixAInput.Text);
                var b = ParseMatrix(MatrixBInput.Text);
                ResultText.Text = "A ∨ B =\n" + MatrixOperations.MatrixToString(MatrixOperations.BooleanAdd(a, b));
            }
            catch (Exception ex) { ResultText.Text = $"Ошибка: {ex.Message}"; }
        }

        private void BooleanMultiply_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var a = ParseMatrix(MatrixAInput.Text);
                var b = ParseMatrix(MatrixBInput.Text);
                ResultText.Text = "A ∧ B =\n" + MatrixOperations.MatrixToString(MatrixOperations.BooleanMultiply(a, b));
            }
            catch (Exception ex) { ResultText.Text = $"Ошибка: {ex.Message}"; }
        }

        private void TransClosure_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var a = ParseMatrix(MatrixAInput.Text);
                ResultText.Text = "Транзитивное замыкание:\n" +
                    MatrixOperations.MatrixToString(MatrixOperations.TransitiveClosure(a));
            }
            catch (Exception ex) { ResultText.Text = $"Ошибка: {ex.Message}"; }
        }
    }
}