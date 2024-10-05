using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public TileType Type { get; private set; }

    [SerializeField] private SpriteRenderer _renderer;

    public void SetType(Sprite sprite, TileType type)
    {
        _renderer.sprite = sprite;
        Type = type;
    }
}

public enum TileType
{
    Normal,
    Grass,
    TallGrass,
    Swamp,
    Water
}
