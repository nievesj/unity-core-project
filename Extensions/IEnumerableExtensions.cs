using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

namespace Core.Common.Extensions.IEnumerable
{
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Gets a random element from an IEnumerable object
        /// </summary>
        /// <param name="source"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns>Random T</returns>
        public static T GetRandomElement<T>(this IEnumerable<T> source)
        {
            return source.ElementAt(Random.Range(0, source.Count()));
        }

        public static IEnumerable<T> GetRandomElements<T>(this IEnumerable<T> list, int elementsCount)
        {
            return list.OrderBy(x => Random.Range(0, list.Count())).Take(elementsCount);
        }

        public static List<T> ToList<T>(this IEnumerable<T> arr)
        {
            var list = new List<T>();
            list.AddRange(arr);
            return list;
        }

        public static IEnumerable<T> Do<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var item in enumerable)
            {
                action?.Invoke(item);
                yield return item;
            }
        }

        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            if (enumerable == null || action == null) return;
            foreach (var item in enumerable) action(item);
        }
    }
}