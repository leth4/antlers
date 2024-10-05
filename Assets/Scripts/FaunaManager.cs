using System.Collections;
using System.Collections.Generic;
using Foundation;
using UnityEngine;

public class FaunaManager : Singleton<FaunaManager>
{
    [SerializeField] private Creature _hunterPrefab;
    [SerializeField] private Creature _deerPrefab;

    private List<Creature> _creatures = new();

    public void GenerateCreatures()
    {
        _creatures.ForEach(creature => { if (creature != null) Destroy(creature.gameObject); });

        for (int i = 0; i < Random.Range(2, 5); i++)
        {
            _creatures.Add(Instantiate(_deerPrefab, GetRandomPositionInBounds(), Quaternion.identity));
        }
        for (int i = 0; i < Random.Range(0, 2); i++)
        {
            _creatures.Add(Instantiate(_hunterPrefab, GetRandomPositionInBounds(), Quaternion.identity));
        }
    }

    private Vector3 GetRandomPositionInBounds()
    {
        return new Vector3(Random.Range(0, 40), Random.Range(0, 20), 0);
    }
}
