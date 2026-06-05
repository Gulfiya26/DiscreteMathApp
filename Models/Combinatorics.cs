using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace TipaDiplom.Models
{
    public static class Combinatorics
    {
        public static BigInteger Factorial(int n)
        {
            if (n < 0) throw new ArgumentException("n должно быть >= 0");
            BigInteger result = 1;
            for (int i = 2; i <= n; i++)
                result *= i;
            return result;
        }

        // Перестановки P(n)
        public static BigInteger Permutations(int n) => Factorial(n);

        // Размещения A(n, k)
        public static BigInteger Arrangements(int n, int k)
        {
            if (k > n) return 0;
            return Factorial(n) / Factorial(n - k);
        }

        // Сочетания C(n, k)
        public static BigInteger Combinations(int n, int k)
        {
            if (k > n) return 0;
            return Factorial(n) / (Factorial(k) * Factorial(n - k));
        }

        // Сочетания с повторениями
        public static BigInteger CombinationsWithRepetition(int n, int k)
        {
            return Combinations(n + k - 1, k);
        }

        // Размещения с повторениями
        public static BigInteger ArrangementsWithRepetition(int n, int k)
        {
            return BigInteger.Pow(n, k);
        }

        // Числа Стирлинга второго рода
        public static BigInteger StirlingSecond(int n, int k)
        {
            if (n == 0 && k == 0) return 1;
            if (n == 0 || k == 0) return 0;
            if (k > n) return 0;

            BigInteger[,] s = new BigInteger[n + 1, k + 1];
            s[0, 0] = 1;

            for (int i = 1; i <= n; i++)
                for (int j = 1; j <= Math.Min(i, k); j++)
                    s[i, j] = j * s[i - 1, j] + s[i - 1, j - 1];

            return s[n, k];
        }

        // Числа Белла
        public static BigInteger BellNumber(int n)
        {
            BigInteger sum = 0;
            for (int k = 0; k <= n; k++)
                sum += StirlingSecond(n, k);
            return sum;
        }

        // Числа Каталана
        public static BigInteger CatalanNumber(int n)
        {
            return Combinations(2 * n, n) / (n + 1);
        }

        // Числа Фибоначчи
        public static BigInteger Fibonacci(int n)
        {
            if (n <= 0) return 0;
            if (n == 1) return 1;
            BigInteger a = 0, b = 1;
            for (int i = 2; i <= n; i++)
            {
                BigInteger temp = a + b;
                a = b;
                b = temp;
            }
            return b;
        }

        // Треугольник Паскаля
        public static BigInteger[,] PascalTriangle(int rows)
        {
            BigInteger[,] triangle = new BigInteger[rows, rows];
            for (int i = 0; i < rows; i++)
            {
                triangle[i, 0] = 1;
                for (int j = 1; j <= i; j++)
                    triangle[i, j] = triangle[i - 1, j - 1] + triangle[i - 1, j];
            }
            return triangle;
        }

        // Генерация всех перестановок
        public static List<List<int>> GeneratePermutations(int n)
        {
            var result = new List<List<int>>();
            var current = Enumerable.Range(1, n).ToList();
            Permute(current, 0, result);
            return result;
        }

        private static void Permute(List<int> arr, int start, List<List<int>> result)
        {
            if (start == arr.Count)
            {
                result.Add(new List<int>(arr));
                return;
            }
            for (int i = start; i < arr.Count; i++)
            {
                (arr[start], arr[i]) = (arr[i], arr[start]);
                Permute(arr, start + 1, result);
                (arr[start], arr[i]) = (arr[i], arr[start]);
            }
        }

        // Принцип включений-исключений
        public static BigInteger InclusionExclusion(List<BigInteger> setSizes,
                                                      List<BigInteger> intersections)
        {
            BigInteger result = 0;
            for (int i = 0; i < setSizes.Count; i++)
                result += setSizes[i];

            int sign = -1;
            for (int i = 0; i < intersections.Count; i++)
            {
                result += sign * intersections[i];
                sign *= -1;
            }
            return result;
        }
    }
}