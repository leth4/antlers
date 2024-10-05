using System;
using System.Collections;
using System.Collections.Generic;
using Foundation;
using UnityEngine;
using Random = UnityEngine.Random;

public class DeerCreature : Creature
{
    private State _state;

    private float _currentTimer;
    private Vector2 _currentDirection;

    private bool _isIdle;

    private void Start()
    {
        SetState(State.Searching);
    }

    private void Update()
    {
        if (_state is State.Searching)
        {
            HandleSearch();
        }
        else if (_state is State.Eating)
        {
        }
        else if (_state is State.Alert)
        {

        }
        else if (_state is State.Running)
        {

        }

        _currentTimer -= Time.deltaTime;
    }

    private void HandleSearch()
    {
        if (!_isIdle) transform.position += _currentDirection.ToVector3() * Speed * Time.deltaTime;

        if (_currentTimer <= 0)
        {
            _isIdle = !_isIdle;
            _currentTimer = _isIdle ? Random.Range(.5f, 1.5f) : Random.Range(3, 5);
            _currentDirection = GetRandomDirection(_currentTimer);
        }

        // SetState(State.Eating);
    }

    private void SetState(State state)
    {
        _state = state;

        if (_state is State.Searching)
        {
            _currentTimer = Random.Range(2, 4);
            _currentDirection = GetRandomDirection(_currentTimer);
        }
        else if (_state is State.Eating)
        {
            _currentTimer = 5;
        }
        else if (_state is State.Alert)
        {
        }
        else if (_state is State.Running)
        {
        }
    }

    private enum State
    {
        Searching,
        Eating,
        Alert,
        Running
    }
}
