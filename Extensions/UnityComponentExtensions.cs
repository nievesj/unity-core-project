using UnityEngine;

public static class UnityComponentExtensions
{
    /// <summary>
    /// Destroy component
    /// </summary>
    /// <param name="component"></param>
    public static void Destroy(this Component component)
    {
        Object.Destroy(component);
    }

    /// <summary>
    /// Destroy gameObject
    /// </summary>
    /// <param name="gameObject"></param>
    public static void Destroy(this GameObject gameObject)
    {
        Object.Destroy(gameObject);
    }
}