using System.Collections;
using System.Collections.Generic;
using Foundation;
using UnityEngine;

public class FaunaManager : Singleton<FaunaManager>
{
    [SerializeField] private Creature _hunterPrefab;
    [SerializeField] private Creature _deerPrefab;
    [SerializeField] private Creature _snakePrefab;

    private List<Creature> _creatures = new();

    public void GenerateCreatures()
    {
        _creatures.ForEach(creature => { if (creature != null) Destroy(creature.gameObject); });

        for (int i = 0; i < Random.Range(3, 6); i++)
        {
            _creatures.Add(Instantiate(_deerPrefab, GetRandomPositionInBounds(), Quaternion.identity));
            _creatures[^1].Initialize();
        }
        for (int i = 0; i < Random.Range(0, 2); i++)
        {
            _creatures.Add(Instantiate(_hunterPrefab, GetRandomPositionInBounds(), Quaternion.identity));
            _creatures[^1].Initialize();
        }
        for (int i = 0; i < Random.Range(0, 3); i++)
        {
            var tile = GridManager.Instance.GetRandomTallGrass();
            if (tile == null) break;
            _creatures.Add(Instantiate(_snakePrefab, tile.transform.position.SetZ(0), Quaternion.identity));
            _creatures[^1].Initialize();
        }
    }

    private Vector3 GetRandomPositionInBounds()
    {
        return new Vector3(Random.Range(0, 35), Random.Range(0, 15), 0);
    }
}
