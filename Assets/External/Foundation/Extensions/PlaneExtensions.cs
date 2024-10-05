using UnityEngine;

namespace Foundation
{
    public static class PlaneExtensions
    {
        public static bool Raycast(this Plane plane, Ray ray, out Vector3 hitPoint)
        {
            var didHit = plane.Raycast(ray, out float enter);
            hitPoint = ray.origin + ray.direction * enter;
            return didHit;
        }
    }
}