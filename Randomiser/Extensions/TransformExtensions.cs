using System.Collections.Generic;
using UnityEngine;

namespace Randomiser;

public static class TransformExtensions
{
    public static IEnumerable<Transform> FindAllChildren(this Transform transform, params string[] names)
    {
        foreach (var name in names)
            yield return transform.Find(name);
    }

    public static Transform EmbedInContainer(this Transform transform)
    {
        var newParent = new GameObject(transform.name + " container").transform;
        newParent.parent = transform.parent;
        newParent.position = transform.position;
        newParent.rotation = transform.rotation;
        newParent.localScale = transform.localScale;
        transform.parent = newParent;
        return newParent;
    }

    public static string GetPath(this Transform transform)
    {
        Transform t = transform;
        List<string> names = new List<string>();

        do
        {
            names.Add(t.name);
            t = t.parent;
        } while (t != null);

        names.Reverse();
        return string.Join("/", names.ToArray());
    }
}
