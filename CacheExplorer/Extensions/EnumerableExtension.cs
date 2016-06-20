using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheExplorer.Extensions
{
    public static class EnumerableExtension
    {
        public static string ConvertHexToString(this IEnumerable<string> hexValues)
        {
            var sb = new StringBuilder();
            foreach (var hex in hexValues)
            {
                var value = Convert.ToInt32(hex, 16);
                sb.Append(Char.ConvertFromUtf32(value));
            }
            return sb.ToString();
        }

        public static string ConvertToString(this IEnumerable<string> list, string delimiter)
        {
            return String.Join(delimiter, list);
        }

        public static IEnumerable<IEnumerable<T>> Split<T>(
            this IEnumerable<T> source, int splitSize)
        {
            using (var enumerator = source.GetEnumerator())
                while (enumerator.MoveNext())
                    yield return YieldSplittedElements(enumerator, splitSize - 1);
        }

        private static IEnumerable<T> YieldSplittedElements<T>(
            IEnumerator<T> source, int splitSize)
        {
            yield return source.Current;
            for (int i = 0; i < splitSize && source.MoveNext(); i++)
                yield return source.Current;
        }
    }
}