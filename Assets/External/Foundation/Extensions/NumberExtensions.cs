using UnityEngine;

namespace Foundation
{
    public static class NumberExtensions
    {
        public static float RoundTo(this float number, float roundTo)
        {
            return Mathf.Round(number / roundTo) * roundTo;
        }
    }
}