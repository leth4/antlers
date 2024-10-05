using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public TileType Type { get; private set; }

    public bool IsTaken;

    [SerializeField] private SpriteRenderer _renderer;

    [SerializeField] private Color _minColorCold;
    [SerializeField] private Color _maxColorCold;

    [SerializeField] private Color _minColorWarm;
    [SerializeField] private Color _maxColorWarm;

    public void SetType(TileType type, int group)
    {
        if (Random.Range(0, 1f) > .8f)
        {
            _renderer.color = Color.Lerp(_minColorCold, _maxColorWarm, Random.Range(0, 1f));
        }
        else if (group % 2 == 0)
        {
            _renderer.color = Color.Lerp(_minColorCold, _maxColorCold, Random.Range(0, 1f));
        }
        else
        {
            _renderer.color = Color.Lerp(_minColorWarm, _maxColorWarm, Random.Range(0, 1f));
        }

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
    Junk
}
