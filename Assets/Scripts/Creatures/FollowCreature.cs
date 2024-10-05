using System.Collections;
using System.Collections.Generic;
using Foundation;
using UnityEngine;

public class FollowCreature : Creature
{
    [SerializeField] private float _speed;

    private void Update()
    {
        transform.position += (PlayerController.Position.ToVector3() - transform.position).normalized * _speed * Time.deltaTime;
    }
}
