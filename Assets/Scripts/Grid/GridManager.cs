using System;
using System.Collections;
using System.Collections.Generic;
using Foundation;
using UnityEngine;
using Random = UnityEngine.Random;

public class GridManager : Singleton<GridManager>
{
    [Header("Settings")]
    [SerializeField] private List<Sprite> _sprites;
    [SerializeField] private Vector2Int _gridSize;
    [SerializeField] private Tile _tilePrefab;

    [Header("Grass")]
    [SerializeField] private float _initialGrassChance;
    [SerializeField] private float _nextGrassChanceDelta;
    [SerializeField] private int _minGrassChunks;
    [SerializeField] private int _maxGrassChunks;

    [Header("Tall Grass")]
    [SerializeField] private float _initialTallGrassChance;
    [SerializeField] private float _nextTallGrassChanceDelta;
    [SerializeField] private int _minTallGrassChunks;
    [SerializeField] private int _maxTallGrassChunks;

    [Header("Swamp")]
    [SerializeField] private float _initialSwampChance;
    [SerializeField] private float _nextSwampChanceDelta;
    [SerializeField] private int _minSwampGrassChunks;
    [SerializeField] private int _maxSwampGrassChunks;

    private Tile[,] _grid;

    public bool IsInBounds(Vector2 position)
    {
        if (position.x < 0 || position.y < 0 || position.x > _gridSize.x || position.y > _gridSize.y) return false;
        return true;
    }

    public Tile FindTile(Vector3 position, TileType tileType)
    {
        var nearestTile = new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
        if (!GridHelper.IsValidCoordinates(_grid, nearestTile)) return null;

        var adjacent = GridHelper.GetAdjacent(_grid, nearestTile.x, nearestTile.y, true);

        foreach (var tile in adjacent)
        {
            if (!tile.IsTaken && tile.Type == tileType) return tile;
        }

        return null;
    }

    public TileType GetTileTypeAt(Vector3 position)
    {
        var nearestTile = new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
        if (!GridHelper.IsValidCoordinates(_grid, nearestTile)) return TileType.None;
        return _grid[nearestTile.x, nearestTile.y].Type;
    }

    public Tile GetTileAt(Vector3 position)
    {
        var nearestTile = new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
        if (!GridHelper.IsValidCoordinates(_grid, nearestTile)) return null;
        return _grid[nearestTile.x, nearestTile.y];
    }

    protected override void Awake()
    {
        base.Awake();
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        _grid = new Tile[_gridSize.x, _gridSize.y];

        GenerateBase();
        GenerateTallGrass();
        GenerateSwamp();
        GenerateGrass();
    }

    private void GenerateSwamp()
    {
        for (int i = 0; i < Random.Range(_minSwampGrassChunks, _maxSwampGrassChunks); i++)
        {
            var coord = GetRandomCoord();
            ApplyToNeighborsRecursive(coord.x, coord.y, _initialSwampChance, _nextSwampChanceDelta, tile => tile.SetType(TileType.Swamp));
        }
    }

    private void GenerateGrass()
    {
        for (int i = 0; i < Random.Range(_minGrassChunks, _maxGrassChunks); i++)
        {
            var coord = GetRandomCoord();
            ApplyToNeighborsRecursive(coord.x, coord.y, _initialGrassChance, _nextGrassChanceDelta, tile => tile.SetType(TileType.Grass));
        }
    }

    private void GenerateTallGrass()
    {
        for (int i = 0; i < Random.Range(_minTallGrassChunks, _maxTallGrassChunks); i++)
        {
            var coord = GetRandomCoord();
            ApplyToNeighborsRecursive(coord.x, coord.y, _initialTallGrassChance, _nextTallGrassChanceDelta, tile => tile.SetType(TileType.TallGrass));
        }
    }

    private void GenerateBase()
    {
        for (int i = 0; i < _gridSize.x; i++)
        {
            for (int j = 0; j < _gridSize.y; j++)
            {
                var tile = Instantiate(_tilePrefab, new Vector3(i, j, 1), Quaternion.identity);
                tile.SetType(TileType.Normal);
                _grid[i, j] = tile;
            }
        }
    }

    private void ApplyToNeighborsRecursive(int x, int y, float chance, float chanceDelta, Action<Tile> applyAction)
    {
        chance -= chanceDelta;

        if (chance <= 0) return;
        if (_grid[x, y] == null) return;

        applyAction?.Invoke(_grid[x, y]);

        var nextCoords = GridHelper.GetAdjacentCoordinates(_grid, x, y, false);

        foreach (var coords in nextCoords)
        {
            if (Random.Range(0, 1f) > chance) continue;
            ApplyToNeighborsRecursive(coords.x, coords.y, chance, chanceDelta, applyAction);
        }
    }

    private Vector2Int GetRandomCoord() => new Vector2Int(Random.Range(0, _gridSize.x), Random.Range(0, _gridSize.y));

    public Sprite GetSprite(TileType type)
    {
        return type switch
        {
            TileType.Normal => _sprites[0],
            TileType.Grass => _sprites[1],
            TileType.TallGrass => _sprites[2],
            TileType.Swamp => _sprites[3],
            TileType.Water => _sprites[4],
            _ => _sprites[0]
        };
    }
}