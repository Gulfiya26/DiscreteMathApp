using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TipaDiplom.Models
{
    public static class BooleanAlgebra
    {
        public static List<Dictionary<string, bool>> GenerateTruthTable(
            List<string> variables, string expression)
        {
            var rows = new List<Dictionary<string, bool>>();
            int count = 1 << variables.Count;

            for (int i = 0; i < count; i++)
            {
                var row = new Dictionary<string, bool>();
                for (int j = 0; j < variables.Count; j++)
                {
                    row[variables[j]] = ((i >> (variables.Count - 1 - j)) & 1) == 1;
                }
                row["Result"] = EvaluateExpression(expression, row);
                rows.Add(row);
            }
            return rows;
        }

        public static bool EvaluateExpression(string expression, Dictionary<string, bool> values)
        {
            
            expression = expression.Replace(" ", "");
            return ParseOr(expression, values, 0, out _);
        }

        private static bool ParseOr(string expr, Dictionary<string, bool> values,
                                      int pos, out int newPos)
        {
            bool result = ParseAnd(expr, values, pos, out pos);
            while (pos < expr.Length && (expr[pos] == '|' || expr[pos] == '+' || expr[pos] == '∨'))
            {
                pos++;
                bool right = ParseAnd(expr, values, pos, out pos);
                result = result || right;
            }
            newPos = pos;
            return result;
        }

        private static bool ParseAnd(string expr, Dictionary<string, bool> values,
                                       int pos, out int newPos)
        {
            bool result = ParseImplication(expr, values, pos, out pos);
            while (pos < expr.Length && (expr[pos] == '&' || expr[pos] == '*' ||
                                          expr[pos] == '∧' || expr[pos] == '·'))
            {
                pos++;
                bool right = ParseImplication(expr, values, pos, out pos);
                result = result && right;
            }
            newPos = pos;
            return result;
        }

        private static bool ParseImplication(string expr, Dictionary<string, bool> values,
                                               int pos, out int newPos)
        {
            bool result = ParseXor(expr, values, pos, out pos);
            while (pos < expr.Length && (expr[pos] == '→' ||
                   (pos + 1 < expr.Length && expr[pos] == '-' && expr[pos + 1] == '>')))
            {
                if (expr[pos] == '-') pos += 2;
                else pos++;
                bool right = ParseXor(expr, values, pos, out pos);
                result = !result || right; // импликация: A→B = ¬A∨B
            }
            newPos = pos;
            return result;
        }

        private static bool ParseXor(string expr, Dictionary<string, bool> values,
                                       int pos, out int newPos)
        {
            bool result = ParseNot(expr, values, pos, out pos);
            while (pos < expr.Length && (expr[pos] == '^' || expr[pos] == '⊕'))
            {
                pos++;
                bool right = ParseNot(expr, values, pos, out pos);
                result = result ^ right;
            }
            newPos = pos;
            return result;
        }

        private static bool ParseNot(string expr, Dictionary<string, bool> values,
                                       int pos, out int newPos)
        {
            if (pos < expr.Length && (expr[pos] == '!' || expr[pos] == '¬' || expr[pos] == '~'))
            {
                pos++;
                bool val = ParseNot(expr, values, pos, out pos);
                newPos = pos;
                return !val;
            }
            return ParseAtom(expr, values, pos, out newPos);
        }

        private static bool ParseAtom(string expr, Dictionary<string, bool> values,
                                        int pos, out int newPos)
        {
            if (pos < expr.Length && expr[pos] == '(')
            {
                pos++; 
                bool result = ParseOr(expr, values, pos, out pos);
                if (pos < expr.Length && expr[pos] == ')') pos++;
                newPos = pos;
                return result;
            }

            if (pos < expr.Length && (expr[pos] == '0' || expr[pos] == '1'))
            {
                newPos = pos + 1;
                return expr[pos] == '1';
            }

            int start = pos;
            while (pos < expr.Length && char.IsLetter(expr[pos]))
                pos++;

            string varName = expr.Substring(start, pos - start);
            newPos = pos;

            if (values.ContainsKey(varName))
                return values[varName];

            throw new Exception($"Неизвестная переменная: {varName}");
        }

        public static string GetSDNF(List<string> variables,
                                       List<Dictionary<string, bool>> truthTable)
        {
            var terms = new List<string>();
            foreach (var row in truthTable.Where(r => r["Result"]))
            {
                var term = string.Join("∧", variables.Select(v =>
                    row[v] ? v : $"¬{v}"));
                terms.Add($"({term})");
            }
            return terms.Count > 0 ? string.Join(" ∨ ", terms) : "0";
        }

        public static string GetSKNF(List<string> variables,
                                       List<Dictionary<string, bool>> truthTable)
        {
            var terms = new List<string>();
            foreach (var row in truthTable.Where(r => !r["Result"]))
            {
                var term = string.Join("∨", variables.Select(v =>
                    row[v] ? $"¬{v}" : v));
                terms.Add($"({term})");
            }
            return terms.Count > 0 ? string.Join(" ∧ ", terms) : "1";
        }

        public static string GetZhegalkinPolynomial(List<string> variables,
                                                      List<Dictionary<string, bool>> truthTable)
        {
            int n = variables.Count;
            int size = 1 << n;
            int[] values = truthTable.Select(r => r["Result"] ? 1 : 0).ToArray();

            int[] coeffs = new int[size];
            Array.Copy(values, coeffs, size);

            for (int i = 0; i < size; i++)
            {
                for (int j = size - 1; j > i; j--)
                {
                    coeffs[j] = coeffs[j] ^ coeffs[j - 1];
                }
            }

            var terms = new List<string>();
            for (int i = 0; i < size; i++)
            {
                if (coeffs[i] == 1)
                {
                    if (i == 0)
                    {
                        terms.Add("1");
                    }
                    else
                    {
                        var vars = new List<string>();
                        for (int j = 0; j < n; j++)
                        {
                            if ((i & (1 << (n - 1 - j))) != 0)
                                vars.Add(variables[j]);
                        }
                        terms.Add(string.Join("·", vars));
                    }
                }
            }
            return terms.Count > 0 ? string.Join(" ⊕ ", terms) : "0";
        }

        public static bool PreservesZero(List<Dictionary<string, bool>> truthTable)
            => !truthTable.First()["Result"];

        public static bool PreservesOne(List<Dictionary<string, bool>> truthTable)
            => truthTable.Last()["Result"];

        public static bool IsSelfDual(List<Dictionary<string, bool>> truthTable)
        {
            int n = truthTable.Count;
            for (int i = 0; i < n / 2; i++)
            {
                if (truthTable[i]["Result"] == truthTable[n - 1 - i]["Result"])
                    return false;
            }
            return true;
        }

        public static bool IsMonotone(List<string> variables,
                                        List<Dictionary<string, bool>> truthTable)
        {
            for (int i = 0; i < truthTable.Count; i++)
            {
                for (int j = i + 1; j < truthTable.Count; j++)
                {
                    bool iLeqJ = variables.All(v => !truthTable[i][v] || truthTable[j][v]);
                    if (iLeqJ && truthTable[i]["Result"] && !truthTable[j]["Result"])
                        return false;
                }
            }
            return true;
        }

        public static bool IsLinear(List<string> variables,
                                      List<Dictionary<string, bool>> truthTable)
        {
            string zhegalkin = GetZhegalkinPolynomial(variables, truthTable);
            return !zhegalkin.Contains("·");
        }

        public static bool IsComplete(List<string> variables,
                                        List<Dictionary<string, bool>> truthTable)
        {
            return !(PreservesZero(truthTable) || PreservesOne(truthTable) ||
                     IsSelfDual(truthTable) || IsMonotone(variables, truthTable) ||
                     IsLinear(variables, truthTable));
        }
    }
}