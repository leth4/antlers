using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public TileType Type { get; private set; }

    public bool IsTaken;

    [SerializeField] private SpriteRenderer _renderer;

    public void SetType(TileType type)
    {
        _renderer.sprite = GridManager.Instance.GetSprite(type);
        Type = type;
    }
}

public enum TileType
{
    None,
    Normal,
    Grass,
    TallGrass,
    Swamp,
    Water
}
