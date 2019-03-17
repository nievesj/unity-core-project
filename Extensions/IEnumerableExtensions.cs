using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
}