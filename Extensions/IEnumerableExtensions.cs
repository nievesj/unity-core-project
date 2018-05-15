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
        var enumerable = source.ToList();
        return enumerable.ElementAt(Random.Range(0, enumerable.Count()));
    }
}