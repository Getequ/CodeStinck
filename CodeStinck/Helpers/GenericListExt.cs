using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Getequ
{
    public static class GenericListExt
    {
        public static IList<T> GetRange<T>(this IList<T> list, int start, int end)
        {
            return list.Skip(start).Take(end - start + 1).ToList();
        }

        public static bool ContainsString(this IList<string> source, string word)
        {
            return source.Any(x => x.IndexOf(word) > -1);
        }

        public static bool ContainsWord(this IList<string> source, string word)
        {
            return source.Any(x => Regex.IsMatch(x, @"\b" + word + @"\b"));
        }

        public static bool ContainsAnyWord(this IList<string> source, IEnumerable<string> words)
        {
            return words.Any(x => source.ContainsWord(x));
        }

        public static bool ContainsAllWord(this IList<string> source, IEnumerable<string> words)
        {
            return words.All(x => source.ContainsWord(x));
        }

        public static IList<string> Replace(this IList<string> source, string old, string @new)
        {
            var result = new List<string>();
            for (int i = 0; i < source.Count; i++)
            {
                result.Add(source[i].Replace(old, @new));
            }
            return result;
        }

        public static IList<string> Replace(this IList<string> source, IDictionary<string, string> pairs)
        {
            var result = new List<string>();
            for (int i = 0; i < source.Count; i++)
            {
                result.Add(source[i].Replace(pairs));
            }
            return result;
        }

        public static IList<string> ReplaceWord(this IList<string> source, string old, string @new)
        {
            var result = new List<string>();
            for (int i = 0; i < source.Count; i++)
            {
                result.Add(source[i].ReplaceWord(old, @new));
            }
            return result;
        }

        public static IList<string> ReplaceWords(this IList<string> source, IDictionary<string, string> pairs)
        {
            var result = new List<string>();
            for (int i = 0; i < source.Count; i++)
            {
                result.Add(source[i].ReplaceWords(pairs));
            }
            return result;
        }

        public static void InsertRange<T>(this IList<T> list, int position, IEnumerable<T> range)
        {
            foreach (var item in range)
                list.Insert(position++, item);
        }
    }
}
