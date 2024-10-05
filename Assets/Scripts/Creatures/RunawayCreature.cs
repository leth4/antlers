using System.Collections;
using System.Collections.Generic;
using Foundation;
using UnityEngine;

public class RunawayCreature : Creature
{
    [SerializeField] private float _speed;

    private void Update()
    {
        transform.position += (transform.position - PlayerController.Position.ToVector3()).normalized * _speed * Time.deltaTime;
    }
}
