using System;
using UnityEngine;


//https://stackoverflow.com/questions/33437244/find-children-of-children-of-a-gameobject

public static class TransformExtensions
{
    public static Transform FindRecursive(this Transform self, string exactName) => self.FindRecursive(child => child.name == exactName);

    public static Transform FindRecursive(this Transform self, Func<Transform, bool> selector)
    {
        foreach (Transform child in self)
        {
            if (selector(child))
            {
                return child;
            }

            var finding = child.FindRecursive(selector);

            if (finding != null)
            {
                return finding;
            }
        }

        return null;
    }
}