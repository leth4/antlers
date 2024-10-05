using UnityEngine;

namespace Foundation
{
    public static class TransformExtensions
    {
        public static void DestroyChildren(this Transform transform)
        {
            foreach (Transform child in transform) Object.Destroy(child.gameObject);
        }
    }
}