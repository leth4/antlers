using System.Collections;
using System.Collections.Generic;
using Foundation;
using UnityEngine;

public class FaunaManager : MonoBehaviour
{
    [SerializeField] private float _despawnDistance;

    [SerializeField] private List<Creature> _creaturePrefabs;

    private void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            // Instantiate(_creaturePrefabs.GetRandomItem(), new Vector3(Random.Range(-5, 5), Random.Range(-5, 5), 0), Quaternion.identity);
            yield return new WaitForSeconds(.2f);
        }
    }
}
