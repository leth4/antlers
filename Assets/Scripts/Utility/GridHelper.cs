using System.Collections.Generic;
using UnityEngine;

public static class GridHelper
{
    public static List<T> GetAdjacent<T>(T[,] matrix, T item, bool diagonal)
    {
        var coords = GetCoordinates(matrix, item);
        if (coords == null) return null;
        return GetAdjacent(matrix, coords.Value.x, coords.Value.y, diagonal);
    }

    public static List<T> GetAdjacent<T>(T[,] matrix, int xCoord, int yCoord, bool diagonal)
    {
        var results = new List<T>();

        for (int x = xCoord - 1; x <= xCoord + 1; x++)
        {
            for (int y = yCoord - 1; y <= yCoord + 1; y++)
            {
                if (x == xCoord && y == yCoord) continue;
                if (!IsValidCoordinates(matrix, x, y)) continue;
                if (!diagonal && x != xCoord && y != yCoord) continue;
                results.Add(matrix[x, y]);
            }
        }

        return results;
    }

    public static List<Vector2Int> GetAdjacentCoordinates<T>(T[,] matrix, int xCoord, int yCoord, bool diagonal)
    {
        var results = new List<Vector2Int>();

        for (int x = xCoord - 1; x <= xCoord + 1; x++)
        {
            for (int y = yCoord - 1; y <= yCoord + 1; y++)
            {
                if (x == xCoord && y == yCoord) continue;
                if (!IsValidCoordinates(matrix, x, y)) continue;
                if (!diagonal && x != xCoord && y != yCoord) continue;
                results.Add(new(x, y));
            }
        }

        return results;
    }
    
    public static Vector2Int? GetCoordinates<T>(T[,] matrix, T item)
    {
        for (int x = 0; x < matrix.GetLength(0); x++)
            for (int y = 0; y < matrix.GetLength(1); y++)
                if (matrix[x, y].Equals(item)) return new Vector2Int(x, y);

        return null;
    }

    public static bool IsValidCoordinates<T>(T[,] matrix, int x, int y)
    {
        return x >= 0 && x < matrix.GetLength(0) && y >= 0 && y < matrix.GetLength(1);
    }

    public static bool IsValidCoordinates<T>(T[,] matrix, Vector2Int coords)
    {
        return IsValidCoordinates(matrix, coords.x, coords.y);
    }
}