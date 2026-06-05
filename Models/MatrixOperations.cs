using System;
using System.Text;

namespace TipaDiplom.Models
{
    public static class MatrixOperations
    {
        public static int[,] Multiply(int[,] a, int[,] b)
        {
            int n = a.GetLength(0);
            int m = b.GetLength(1);
            int p = a.GetLength(1);
            var result = new int[n, m];

            for (int i = 0; i < n; i++)
                for (int j = 0; j < m; j++)
                    for (int k = 0; k < p; k++)
                        result[i, j] += a[i, k] * b[k, j];
            return result;
        }

        public static int[,] Add(int[,] a, int[,] b)
        {
            int n = a.GetLength(0);
            int m = a.GetLength(1);
            var result = new int[n, m];

            for (int i = 0; i < n; i++)
                for (int j = 0; j < m; j++)
                    result[i, j] = a[i, j] + b[i, j];
            return result;
        }

        public static int[,] Transpose(int[,] matrix)
        {
            int n = matrix.GetLength(0);
            int m = matrix.GetLength(1);
            var result = new int[m, n];

            for (int i = 0; i < n; i++)
                for (int j = 0; j < m; j++)
                    result[j, i] = matrix[i, j];
            return result;
        }

        public static int[,] Power(int[,] matrix, int power)
        {
            int n = matrix.GetLength(0);
            var result = new int[n, n];
            // Единичная матрица
            for (int i = 0; i < n; i++) result[i, i] = 1;

            for (int p = 0; p < power; p++)
                result = Multiply(result, matrix);
            return result;
        }

        // Булево умножение
        public static int[,] BooleanMultiply(int[,] a, int[,] b)
        {
            int n = a.GetLength(0);
            int m = b.GetLength(1);
            int p = a.GetLength(1);
            var result = new int[n, m];

            for (int i = 0; i < n; i++)
                for (int j = 0; j < m; j++)
                    for (int k = 0; k < p; k++)
                        if (a[i, k] == 1 && b[k, j] == 1)
                        {
                            result[i, j] = 1;
                            break;
                        }
            return result;
        }

        // Булево сложение
        public static int[,] BooleanAdd(int[,] a, int[,] b)
        {
            int n = a.GetLength(0);
            int m = a.GetLength(1);
            var result = new int[n, m];

            for (int i = 0; i < n; i++)
                for (int j = 0; j < m; j++)
                    result[i, j] = (a[i, j] == 1 || b[i, j] == 1) ? 1 : 0;
            return result;
        }

        // Определитель (для небольших матриц)
        public static double Determinant(int[,] matrix)
        {
            int n = matrix.GetLength(0);
            double[,] m = new double[n, n];
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    m[i, j] = matrix[i, j];

            return Det(m, n);
        }

        private static double Det(double[,] m, int n)
        {
            if (n == 1) return m[0, 0];
            if (n == 2) return m[0, 0] * m[1, 1] - m[0, 1] * m[1, 0];

            double det = 0;
            for (int j = 0; j < n; j++)
            {
                var minor = GetMinor(m, 0, j, n);
                det += (j % 2 == 0 ? 1 : -1) * m[0, j] * Det(minor, n - 1);
            }
            return det;
        }

        private static double[,] GetMinor(double[,] m, int row, int col, int n)
        {
            var minor = new double[n - 1, n - 1];
            int mi = 0;
            for (int i = 0; i < n; i++)
            {
                if (i == row) continue;
                int mj = 0;
                for (int j = 0; j < n; j++)
                {
                    if (j == col) continue;
                    minor[mi, mj] = m[i, j];
                    mj++;
                }
                mi++;
            }
            return minor;
        }

        // Транзитивное замыкание (алгоритм Уоршелла)
        public static int[,] TransitiveClosure(int[,] matrix)
        {
            int n = matrix.GetLength(0);
            int[,] result = (int[,])matrix.Clone();

            for (int k = 0; k < n; k++)
                for (int i = 0; i < n; i++)
                    for (int j = 0; j < n; j++)
                        if (result[i, k] == 1 && result[k, j] == 1)
                            result[i, j] = 1;
            return result;
        }

        public static string MatrixToString(int[,] matrix)
        {
            var sb = new StringBuilder();
            int n = matrix.GetLength(0);
            int m = matrix.GetLength(1);

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    sb.Append(matrix[i, j].ToString().PadLeft(5));
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}