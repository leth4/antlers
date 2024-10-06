using System;
using System.Collections;
using System.Collections.Generic;
using Foundation;
using UnityEngine;
using Random = UnityEngine.Random;

public class FaunaManager : Singleton<FaunaManager>
{
    public static event Action OnNoDeersLeft;

    [SerializeField] private Creature _hunterPrefab;
    [SerializeField] private Creature _deerPrefab;
    [SerializeField] private Creature _snakePrefab;

    private bool _detectingEnding;

    private List<Creature> _creatures = new();

    public void GenerateCreatures()
    {
        _creatures.ForEach(creature => { if (creature != null) Destroy(creature.gameObject); });

        for (int i = 0; i < Random.Range(3, 6); i++)
        {
            _creatures.Add(Instantiate(_deerPrefab, GetRandomPositionInBounds(6), Quaternion.identity));
            _creatures[^1].Initialize();
        }
        for (int i = 0; i < Random.Range(0, 2); i++)
        {
            _creatures.Add(Instantiate(_hunterPrefab, GetRandomPositionInBounds(7), Quaternion.identity));
            _creatures[^1].Initialize();
        }
        for (int i = 0; i < Random.Range(0, 3); i++)
        {
            for (int j = 0; j < 30; j++)
            {
                var tile = GridManager.Instance.GetRandomTallGrass();
                if (tile == null) break;
                if ((GridManager.Instance.GetTileTypeAt(PlayerController.Instance.transform.position) is TileType.TallGrass) && Vector3.Distance(tile.transform.position, PlayerController.Instance.transform.position) < 8) continue;
                _creatures.Add(Instantiate(_snakePrefab, tile.transform.position.SetZ(0), Quaternion.identity));
                _creatures[^1].Initialize();
                break;
            }
        }
        _detectingEnding = true;
    }

    private Vector3 GetRandomPositionInBounds(float minDistanceToPlayer)
    {
        for (int i = 0; i < 30; i++)
        {
            var position = new Vector3(Random.Range(0, 35), Random.Range(0, 15), 0);
            if (Vector3.Distance(position, PlayerController.Instance.transform.position) > minDistanceToPlayer) return position;
        }
        Debug.LogWarning("Can't find a position");
        return Vector3.zero;
    }

    private void HandleCreatureDied(Creature deadCreature)
    {
        if (!_detectingEnding) return;
        foreach (var creature in _creatures)
        {
            if (creature != null && creature != deadCreature && creature is DeerCreature) return;
        }
        OnNoDeersLeft?.Invoke();
        _detectingEnding = false;
    }

    private void OnEnable()
    {
        Creature.OnDied += HandleCreatureDied;
    }

    private void OnDisable()
    {
        Creature.OnDied += HandleCreatureDied;
    }
}
