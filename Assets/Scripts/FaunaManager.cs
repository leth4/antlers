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
    [SerializeField] private Creature _mulePrefab;

    private bool _detectingEnding;

    private List<Creature> _creatures = new();

    public void GenerateCreatures()
    {
        _detectingEnding = false;
        _creatures.ForEach(creature => { if (creature != null) Destroy(creature.gameObject); });
        _creatures = new();

        var haveMules = Random.Range(0, 1f) < .4f;
        var haveHunter = Random.Range(0, 1f) < .7f;

        for (int i = 0; i < Random.Range(3, 7); i++)
        {
            _creatures.Add(Instantiate(_deerPrefab, GetRandomPositionInBounds(6), Quaternion.identity));
            _creatures[^1].Initialize();
        }
        if (haveHunter) for (int i = 0; i < 1; i++)
            {
                _creatures.Add(Instantiate(_hunterPrefab, GetRandomPositionInBounds(7), Quaternion.identity));
                _creatures[^1].Initialize();
            }
        if (haveMules) for (int i = 0; i < 1; i++)
            {
                _creatures.Add(Instantiate(_mulePrefab, GetRandomPositionInBounds(6), Quaternion.identity));
                _creatures[^1].Initialize();
            }
        for (int i = 0; i < Random.Range(0, 3); i++)
        {
            for (int j = 0; j < 30; j++)
            {
                var tile = GridManager.Instance.GetRandomTallGrass();
                if (tile == null) break;
                if (Vector3.Distance(tile.transform.position.SetZ(0), PlayerController.Instance.transform.position) < 8) continue;
                _creatures.Add(Instantiate(_snakePrefab, tile.transform.position.SetZ(0), Quaternion.identity));
                _creatures[^1].Initialize();
                break;
            }
        }
        _detectingEnding = true;
    }

    public void DestroyCreatures()
    {
        _detectingEnding = false;
        _creatures.ForEach(creature => { if (creature != null) Destroy(creature.gameObject); });
        _creatures = new();
    }

    private Vector3 GetRandomPositionInBounds(float minDistanceToPlayer)
    {
        for (int i = 0; i < 30; i++)
        {
            var position = new Vector3(Random.Range(1, 35), Random.Range(1, 15), 0);
            if (Vector3.Distance(position, PlayerController.Instance.transform.position) > minDistanceToPlayer) return position;
        }
        Debug.LogWarning("Can't find a position");
        return Vector3.zero;
    }

    private void HandleDeerDied()
    {
        if (!_detectingEnding) return;
        if (FindObjectsOfType<DeerCreature>().Length > 1) return;

        OnNoDeersLeft?.Invoke();
        _detectingEnding = false;
    }

    private void OnEnable()
    {
        Creature.OnDied += HandleDeerDied;
    }

    private void OnDisable()
    {
        Creature.OnDied += HandleDeerDied;
    }
}
