using System.Collections.Generic;
using System.Linq;

namespace Foundation
{
    public static class EnumerableExtensions
    {
        public static string ToString<T>(this IEnumerable<T> list, string separator)
        {
            string result = "";
            foreach (var obj in list)
            {
                if (result != "") result += separator;
                result += obj.ToString();
            }
            return result;
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> list)
        {
            var random = new System.Random();
            return list.OrderBy(x => (random.Next()));
        }

        public static T GetRandomItem<T>(this IEnumerable<T> list)
        {
            var random = new System.Random();
            int index = random.Next(0, list.Count());
            return list.ElementAt(index);
        }

        public static bool IsEmpty<T>(this IEnumerable<T> list)
        {
            return list.Count() == 0;
        }
    }
}