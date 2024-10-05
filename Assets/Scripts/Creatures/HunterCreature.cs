using System;
using System.Collections;
using System.Collections.Generic;
using Foundation;
using UnityEngine;

public class HunterCreature : Creature
{
    [SerializeField] private float _searchDistance;
    [SerializeField] private float _followDistance;

    [SerializeField] private float _followSpeed;
    [SerializeField] private float _wanderSpeed;

    private Creature _target;

    private Vector3 _wanderDirection;

    private void Start()
    {
        _wanderDirection = UnityEngine.Random.insideUnitCircle.normalized;
    }

    private void Update()
    {
        if (_target == null)
        {
            SearchForTarget();
        }
        else
        {
            Hunt();
        }
    }

    private void Hunt()
    {
        if (Vector3.Distance(transform.position, _target.transform.position) > _followDistance)
        {
            _wanderDirection = (_target.transform.position - transform.position).normalized;
            _target = null;
            return;
        }

        transform.position += (_target.transform.position - transform.position).normalized * _followSpeed * Time.deltaTime;
    }
    

    private void SearchForTarget()
    {
        transform.position += _wanderDirection * _wanderSpeed * Time.deltaTime;

        var runaway = FindCreature(transform.position, _searchDistance, Strength - 1);
        if (runaway != null)
        {
            _target = runaway;
        }
    }
}
