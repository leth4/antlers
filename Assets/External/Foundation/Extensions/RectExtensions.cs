using UnityEngine;

namespace Foundation
{
    public static class RectExtensions
    {
        public static Rect SetX(this Rect rect, float x)
        {
            return new Rect(x, rect.y, rect.width, rect.height);
        }

        public static Rect SetY(this Rect rect, float y)
        {
            return new Rect(rect.x, y, rect.width, rect.height);
        }

        public static Rect SetWidth(this Rect rect, float width)
        {
            return new Rect(rect.x, rect.y, width, rect.height);
        }

        public static Rect SetHeight(this Rect rect, float height)
        {
            return new Rect(rect.x, rect.y, rect.width, height);
        }
    }
}