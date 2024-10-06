using System;
using System.Collections;
using System.Collections.Generic;
using Foundation;
using Unity.Collections;
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

    [Header("Mines")]
    [SerializeField] private int _minMines;
    [SerializeField] private int _maxMines;

    private Tile[,] _grid;
    private int _group;

    private List<Tile> _tallGrass;

    public bool IsInBounds(Vector2 position)
    {
        if (position.x < 0 || position.y < 0 || position.x > _gridSize.x || position.y > _gridSize.y) return false;
        return true;
    }

    public Tile FindTile(Vector3 position, TileType tileType)
    {
        var nearestTile = new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
        if (!GridHelper.IsValidCoordinates(_grid, nearestTile)) return null;

        var adjacentCoords = GridHelper.GetAdjacentCoordinates(_grid, nearestTile.x, nearestTile.y, true);

        adjacentCoords.Shuffle();

        foreach (var coords in adjacentCoords)
        {
            var nextAdjacent = GridHelper.GetAdjacent(_grid, coords.x, coords.y, true);
            nextAdjacent.Shuffle();
            foreach (var tile in nextAdjacent)
            {
                if (tile == _grid[nearestTile.x, nearestTile.y]) continue;
                if (!tile.IsTaken && tile.Type == tileType) return tile;
            }
        }

        return null;
    }

    public Tile GetRandomTallGrass()
    {
        _tallGrass.Shuffle();
        for (int i = _tallGrass.Count - 1; i >= 0; i--)
        {
            if (_tallGrass[i].Type is TileType.TallGrass) return _tallGrass[i];
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

    public void GenerateGrid()
    {
        transform.DestroyChildren();

        _grid = new Tile[_gridSize.x, _gridSize.y];
        _group = 0;

        GenerateBase();
        GenerateSwamp();
        GenerateTallGrass();
        GenerateGrass();

        GenerateJunk();

        GenerateMines();
    }

    private void GenerateMines()
    {
        var count = Random.Range(_minMines, _maxMines);
        for (int i = 0; i < count; i++)
        {
            _group++;
            var coord = GetRandomCoord();
            if (Vector3.Distance(new Vector3(coord.x, coord.y, 0), PlayerController.Instance.transform.position) < 3)
            {
                i--;
                continue;
            }
            _grid[coord.x, coord.y].SetType(TileType.Mine, _group);
            var adjacent = GridHelper.GetAdjacent(_grid, _grid[coord.x, coord.y], true);
            adjacent.Shuffle();
            if (Random.Range(0, 1f) > .4f) adjacent[0].SetType(TileType.Mine, _group);
            if (Random.Range(0, 1f) > .7f) adjacent[1].SetType(TileType.Mine, _group);
        }
    }

    private void GenerateJunk()
    {
        for (int i = 0; i < Random.Range(3, 6); i++)
        {
            _group++;
            var coord = GetRandomCoord();
            ApplyToNeighborsRecursive(coord.x, coord.y, .8f, .1f, tile => tile.SetType(TileType.Junk, _group));
        }
        for (int i = 0; i < Random.Range(3, 6); i++)
        {
            _group++;
            var coord = GetRandomCoord();
            DrunkardWalkRecursive(coord.x, coord.y, 1, .005f, tile => tile.SetType(TileType.Junk, _group));
        }
    }

    private void GenerateBase()
    {
        for (int i = 0; i < _gridSize.x; i++)
        {
            for (int j = 0; j < _gridSize.y; j++)
            {
                var tile = Instantiate(_tilePrefab, new Vector3(i, j, 1), Quaternion.identity);
                tile.transform.SetParent(transform);
                tile.SetType(TileType.Normal, 0);
                _grid[i, j] = tile;
            }
        }
    }

    private void GenerateSwamp()
    {
        for (int i = 0; i < Random.Range(_minSwampGrassChunks, _maxSwampGrassChunks); i++)
        {
            _group++;
            var coord = GetRandomCoord();
            ApplyToNeighborsRecursive(coord.x, coord.y, _initialSwampChance, _nextSwampChanceDelta, tile => tile.SetType(TileType.Swamp, _group));
        }
    }

    private void GenerateGrass()
    {
        for (int i = 0; i < Random.Range(_minGrassChunks, _maxGrassChunks); i++)
        {
            _group++;
            var coord = GetRandomCoord();
            ApplyToNeighborsRecursive(coord.x, coord.y, _initialGrassChance, _nextGrassChanceDelta, tile => tile.SetType(TileType.Grass, _group));
        }
    }

    private void GenerateTallGrass()
    {
        _tallGrass = new();
        for (int i = 0; i < Random.Range(_minTallGrassChunks, _maxTallGrassChunks); i++)
        {
            _group++;
            var coord = GetRandomCoord();
            ApplyToNeighborsRecursive(coord.x, coord.y, _initialTallGrassChance, _nextTallGrassChanceDelta, tile =>
            {
                _tallGrass.Add(tile);
                tile.SetType(TileType.TallGrass, _group);
            });
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

    private void DrunkardWalkRecursive(int x, int y, float chance, float chanceDelta, Action<Tile> applyAction)
    {
        chance -= chanceDelta;

        if (Random.Range(0, 1f) > chance) return;
        if (_grid[x, y] == null) return;

        applyAction?.Invoke(_grid[x, y]);

        var nextCoords = new Vector2Int[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right }.GetRandomItem();

        nextCoords = new Vector2Int(nextCoords.x + x, nextCoords.y + y);

        if (GridHelper.IsValidCoordinates(_grid, nextCoords))
        {
            DrunkardWalkRecursive(nextCoords.x, nextCoords.y, chance, chanceDelta, applyAction);
        }
        else
        {
            DrunkardWalkRecursive(x, y, chance, chanceDelta, applyAction);
        }
    }

    private Vector2Int GetRandomCoord() => new Vector2Int(Random.Range(0, _gridSize.x), Random.Range(0, _gridSize.y));

    public Sprite GetSprite(TileType type)
    {
        return type switch
        {
            TileType.Normal => _sprites[0],
            TileType.Grass => _sprites[Random.Range(1, 3)],
            TileType.TallGrass => _sprites[3],
            TileType.Swamp => _sprites[4],
            TileType.Junk => _sprites[Random.Range(5, 8)],
            TileType.Mine => _sprites[8],
            _ => _sprites[0]
        };
    }
}