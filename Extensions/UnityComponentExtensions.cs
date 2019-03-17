using System.Collections.Generic;
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
    
    public static void DestroyImmediate(this Component component)
    {
        Object.DestroyImmediate(component);
    }

    /// <summary>
    /// Destroy gameObject
    /// </summary>
    /// <param name="gameObject"></param>
    public static void Destroy(this GameObject gameObject)
    {
        Object.Destroy(gameObject);
    }
    
    public static void DestroyImmediate(this GameObject gameObject)
    {
        Object.DestroyImmediate(gameObject);
    }

    /// <summary>
    /// Destroy gameObject
    /// </summary>
    public static void DestroyGameObject(this Component component)
    {
        Object.Destroy(component.gameObject);
    }
    
    public static void DestroyImmediateGameObject(this Component component)
    {
        Object.DestroyImmediate(component.gameObject);
    }

    /// <summary>
    /// Same as GameObject.SetActive(), but applied from a component
    /// </summary>
    /// <param name="component"></param>
    /// <param name="isActive"></param>
    public static void SetActive(this Component component, bool isActive)
    {
        component.gameObject.SetActive(isActive);
    }

    /// <summary>
    /// Add a child to the gameobject. Same as SetParent on other.
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="other"></param>
    /// <param name="worldPositionStays">If true, the parent-relative position, scale and
    /// rotation are modified such that the object keeps the same world space position,
    /// rotation and scale as before.</param>
    public static void AddChild(this GameObject gameObject, GameObject other,bool worldPositionStays = true)
    {
        gameObject.transform.AddChild(other.transform, worldPositionStays);
    }

    /// <summary>
    /// Add a child to the gameobject. Same as SetParent on other.
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="other"></param>
    /// <param name="worldPositionStays">If true, the parent-relative position, scale and
    /// rotation are modified such that the object keeps the same world space position,
    /// rotation and scale as before.</param>
    public static void AddChild(this GameObject gameObject, Component other, bool worldPositionStays = true)
    {
        gameObject.transform.AddChild(other, worldPositionStays);
    }

    /// <summary>
    /// Add a child to the component.  Same as SetParent on other.
    /// </summary>
    /// <param name="component"></param>
    /// <param name="other"></param>
    /// <param name="worldPositionStays">If true, the parent-relative position, scale and
    /// rotation are modified such that the object keeps the same world space position,
    /// rotation and scale as before.</param>
    public static void AddChild(this Component component, Component other, bool worldPositionStays = true)
    {
        other.transform.SetParent(component.transform, worldPositionStays);
    }

    /// <summary>
    /// Get all children from tranform. If any.
    /// </summary>
    /// <param name="transform"></param>
    /// <returns></returns>
    public static List<Transform> GetAllChildren(this Transform transform)
    {
        var children = new List<Transform>();
        foreach(Transform child in transform)
            children.Add(child);

        return children;
    }

    /// <summary>
    /// Detach GameObject from parent
    /// </summary>
    /// <param name="gameObject"></param>
    public static void DetachFromParent(this GameObject gameObject)
    {
        gameObject.transform.parent = null;
    }
    
    /// <summary>
    /// Detach component from parent
    /// </summary>
    /// <param name="component"></param>
    public static void DetachFromParent(this Component component)
    {
        component.transform.parent = null;
    }

    /// <summary>
    /// Distance between two objects. Same as Vector3.Distance
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="useLocalPosition"></param>
    /// <returns></returns>
    public static float Distance(this Component from, Component to, bool useLocalPosition = false)
    {
        return useLocalPosition ? Vector3.Distance(from.transform.localPosition, to.transform.localPosition) : 
            Vector3.Distance(from.transform.position, to.transform.position);
    }
    
    /// <summary>
    /// Distance between two objects. Same as Vector3.Distance
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="useLocalPosition"></param>
    /// <returns></returns>
    public static float Distance(this Component from, Vector3 to, bool useLocalPosition = false)
    {
        return useLocalPosition ? Vector3.Distance(from.transform.localPosition, to) : 
            Vector3.Distance(from.transform.position, to);
    }

    public static Vector3 Position(this Component comp, bool useLocalPosition = false)
    {
        return useLocalPosition ? comp.transform.localPosition : comp.transform.position;
    }

    public static IEnumerable<Vector3> RadiusPoints(this Component from, float radius, float angle, int segments)
    {
        var ret = new List<Vector3>();
        for (var i = 0; i < segments; i++)
        {
            var x = from.Position().x + Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            var z = from.Position().z + Mathf.Cos(Mathf.Deg2Rad * angle) * radius;

            ret.Add(new Vector3(x, from.transform.position.y, z));
            angle += (360f / segments);
        }

        return ret;
    }

    /// <summary>
    /// Distance between two objects 2D. Same as Vector2.Distance.
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="useLocalPosition"></param>
    /// <returns></returns>
    public static float Distance2D(this Component from, Component to, bool useLocalPosition = false)
    {
        return useLocalPosition ? Vector2.Distance(from.transform.localPosition, to.transform.localPosition) : 
            Vector2.Distance(from.transform.position, to.transform.position);
    }
    
    /// <summary>
    /// Distance between two objects 2D. Same as Vector2.Distance.
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="useLocalPosition"></param>
    /// <returns></returns>
    public static float Distance2D(this Component from, Vector2 to, bool useLocalPosition = false)
    {
        return useLocalPosition ? Vector2.Distance(from.transform.localPosition, to) : 
            Vector2.Distance(from.transform.position, to);
    }
    
    public static Vector3 Position2D(this Component comp, bool useLocalPosition = false)
    {
        return useLocalPosition ? comp.transform.localPosition : comp.transform.position;
    }
    
}