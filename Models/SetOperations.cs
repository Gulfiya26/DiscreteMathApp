using System;
using System.Collections.Generic;
using System.Linq;

namespace TipaDiplom.Models
{
    public static class SetOperations
    {
        public static HashSet<int> Parse(string input)
        {
            var set = new HashSet<int>();
            if (string.IsNullOrWhiteSpace(input)) return set;

            foreach (var item in input.Split(new[] { ',', ' ', ';' },
                         StringSplitOptions.RemoveEmptyEntries))
            {
                if (int.TryParse(item.Trim(), out int value))
                    set.Add(value);
            }
            return set;
        }

        public static HashSet<int> Union(HashSet<int> a, HashSet<int> b)
        {
            var result = new HashSet<int>(a);
            result.UnionWith(b);
            return result;
        }

        public static HashSet<int> Intersection(HashSet<int> a, HashSet<int> b)
        {
            var result = new HashSet<int>(a);
            result.IntersectWith(b);
            return result;
        }

        public static HashSet<int> Difference(HashSet<int> a, HashSet<int> b)
        {
            var result = new HashSet<int>(a);
            result.ExceptWith(b);
            return result;
        }

        public static HashSet<int> SymmetricDifference(HashSet<int> a, HashSet<int> b)
        {
            var result = new HashSet<int>(a);
            result.SymmetricExceptWith(b);
            return result;
        }

        public static HashSet<int> Complement(HashSet<int> a, HashSet<int> universal)
        {
            var result = new HashSet<int>(universal);
            result.ExceptWith(a);
            return result;
        }

        public static List<(int, int)> CartesianProduct(HashSet<int> a, HashSet<int> b)
        {
            return a.SelectMany(x => b, (x, y) => (x, y)).ToList();
        }

        public static List<HashSet<int>> PowerSet(HashSet<int> set)
        {
            var elements = set.ToList();
            var result = new List<HashSet<int>>();
            int count = 1 << elements.Count;

            for (int i = 0; i < count; i++)
            {
                var subset = new HashSet<int>();
                for (int j = 0; j < elements.Count; j++)
                {
                    if ((i & (1 << j)) != 0)
                        subset.Add(elements[j]);
                }
                result.Add(subset);
            }
            return result;
        }

        public static string SetToString(HashSet<int> set)
        {
            return "{" + string.Join(", ", set.OrderBy(x => x)) + "}";
        }

        public static bool IsSubset(HashSet<int> a, HashSet<int> b)
            => a.IsSubsetOf(b);

        public static bool IsProperSubset(HashSet<int> a, HashSet<int> b)
            => a.IsProperSubsetOf(b);
    }
}