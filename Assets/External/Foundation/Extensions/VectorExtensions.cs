using UnityEngine;

namespace Foundation
{
    public static class VectorExtensions
    {
        public static Vector3 SetX(this Vector3 v, float x)
        {
            return new Vector3(x, v.y, v.z);
        }

        public static Vector3 SetY(this Vector3 v, float y)
        {
            return new Vector3(v.x, y, v.z);
        }

        public static Vector3 SetZ(this Vector3 v, float z)
        {
            return new Vector3(v.x, v.y, z);
        }

        public static Vector3 Abs(this Vector3 v)
        {
            return new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
        }

        public static Vector2 ToVector2(this Vector3 v)
        {
            return new Vector2(v.x, v.y);
        }

        public static Vector2 SetX(this Vector2 v, float x)
        {
            return new Vector2(x, v.y);
        }

        public static Vector2 SetY(this Vector2 v, float y)
        {
            return new Vector2(v.x, y);
        }

        public static Vector2 Abs(this Vector2 v)
        {
            return new Vector2(Mathf.Abs(v.x), Mathf.Abs(v.y));
        }

        public static Vector3 ToVector3(this Vector2 v, float z = 0)
        {
            return new Vector3(v.x, v.y, z);
        }

        public static Vector3Int SetX(this Vector3Int v, int x)
        {
            return new Vector3Int(x, v.y, v.z);
        }

        public static Vector3Int SetY(this Vector3Int v, int y)
        {
            return new Vector3Int(v.x, y, v.z);
        }

        public static Vector3Int SetZ(this Vector3Int v, int z)
        {
            return new Vector3Int(v.x, v.y, z);
        }

        public static Vector3Int Abs(this Vector3Int v)
        {
            return new Vector3Int(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
        }

        public static Vector2Int ToVector2Int(this Vector3Int v)
        {
            return new Vector2Int(v.x, v.y);
        }

        public static Vector2Int SetX(this Vector2Int v, int x)
        {
            return new Vector2Int(x, v.y);
        }

        public static Vector2Int SetY(this Vector2Int v, int y)
        {
            return new Vector2Int(v.x, y);
        }

        public static Vector2Int Abs(this Vector2Int v)
        {
            return new Vector2Int(Mathf.Abs(v.x), Mathf.Abs(v.y));
        }

        public static Vector3Int ToVector3Int(this Vector2Int v, int z = 0)
        {
            return new Vector3Int(v.x, v.y, z);
        }
    }
}