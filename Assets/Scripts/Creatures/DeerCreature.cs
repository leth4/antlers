using System;
using System.Collections;
using System.Collections.Generic;
using Foundation;
using UnityEngine;
using Random = UnityEngine.Random;

public class DeerCreature : Creature
{
    [SerializeField] private float _alertRadius;
    [SerializeField] private float _alertTime;
    [SerializeField] private float _runningSpeed;

    private State _state;

    private float _currentTimer;
    private Tile _currentTarget;

    private Transform _currentPredatorTransform;

    private void Start()
    {
        SetState(State.Searching);
    }

    private void Update()
    {
        if (_state != State.Alert && _state != State.Running)
        {
            var predator = GetNearbyPredator(_alertRadius);
            if (predator != null)
            {
                _currentPredatorTransform = predator;
                SetState(State.Alert);
                return;
            }
        }

        if (_state is State.Searching)
        {
            HandleSearch();
        }
        else if (_state is State.Idle)
        {
            HandleIdle();
        }
        else if (_state is State.Eating)
        {
            HandleEating();
        }
        else if (_state is State.Alert)
        {
            HandleAlert();
        }
        else if (_state is State.Running)
        {
            HandleRunning();
        }

        _currentTimer -= Time.deltaTime;
    }

    private void HandleRunning()
    {
        transform.position += (transform.position - _currentPredatorTransform.position).normalized * _runningSpeed * GetSpeedModifier() * Time.deltaTime;

        if (!GridManager.Instance.IsInBounds(transform.position)) Die();
    }

    private void HandleAlert()
    {
        if (!GetNearbyPredator(_alertRadius)) SetState(State.Searching);
        if (_currentTimer <= 0) SetState(State.Running);
    }

    private void HandleEating()
    {
        if (_currentTimer <= 0)
        {
            _currentTarget.SetType(TileType.Normal);
            SetState(State.Searching);
        }
    }

    private void HandleSearch()
    {
        transform.position = Vector3.MoveTowards(transform.position, _currentTarget.transform.position.SetZ(0), Speed * GetSpeedModifier() * Time.deltaTime);

        if (IsAtTarget())
        {
            if (_currentTarget.Type is TileType.Grass) SetState(State.Eating);
            else SetState(State.Idle);
        }
    }

    private void HandleIdle()
    {
        if (_currentTimer <= 0)
        {
            SetState(State.Searching);
        }
    }

    private void SetState(State state)
    {
        _state = state;

        if (_state is State.Searching)
        {
            if (_currentTarget != null) _currentTarget.IsTaken = false;

            var grassTile = GridManager.Instance.FindTile(transform.position, TileType.Grass);
            if (grassTile != null) _currentTarget = grassTile;
            else _currentTarget = GetRandomTarget(Random.Range(2, 4));

            _currentTarget.IsTaken = true;
        }
        if (_state is State.Idle)
        {
            _currentTimer = Random.Range(1, 2);

            if (_currentTarget != null) _currentTarget.IsTaken = true;
        }
        else if (_state is State.Eating)
        {
            _currentTimer = 3;

            if (_currentTarget != null) _currentTarget.IsTaken = true;
        }
        else if (_state is State.Alert)
        {
            _currentTimer = _alertTime;
        }
        else if (_state is State.Running)
        {
            if (_currentTarget != null) _currentTarget.IsTaken = false;
        }
    }

    private bool IsAtTarget()
    {
        if (_currentTarget == null) return false;
        return Vector3.SqrMagnitude(_currentTarget.transform.position.SetZ(0) - transform.position) < .0001f;
    }

    private float GetSpeedModifier()
    {
        var tile = GridManager.Instance.GetTileTypeAt(transform.position);
        if (tile is TileType.TallGrass) return .8f;
        if (tile is TileType.Swamp) return .4f;
        return 1;
    }

    private enum State
    {
        Searching,
        Idle,
        Eating,
        Alert,
        Running
    }
}
