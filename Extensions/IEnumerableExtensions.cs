using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

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
        Random.InitState(DateTime.Now.Millisecond);
        return source.ElementAt(Random.Range(0, source.Count()));
    }

    public static List<T> ToList<T>(this IEnumerable<T> arr)
    {
        var list = new List<T>();
        list.AddRange(arr);
        return list;
    }
}