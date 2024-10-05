using System;
using System.Collections;
using System.Collections.Generic;
using Foundation;
using UnityEngine;
using Random = UnityEngine.Random;

public class HunterCreature : Creature
{
    [SerializeField] private float _searchDistance;
    [SerializeField] private float _followDistance;

    [SerializeField] private float _alertTime;

    [SerializeField] private float _runningSpeed;

    private Tile _tileTarget;
    private Creature _creatureTarget;
    private float _currentTimer;

    private bool _followingPlayer;

    private State _state;

    private void Start()
    {
        SetState(State.Searching);
    }

    private void Update()
    {
        if (_state != State.Alert && _state != State.Running)
        {
            if (CanHearPlayer(_searchDistance))
            {
                SetState(State.Alert);
                _followingPlayer = true;
                return;
            }

            var creature = FindCreature(_searchDistance, 99);
            if (creature != null)
            {
                SetState(State.Alert);
                _creatureTarget = creature;
                return;
            }
        }

        if (_state is State.Searching)
        {
            transform.position = Vector3.MoveTowards(transform.position, _tileTarget.transform.position.SetZ(0), Speed * GetSpeedModifier() * Time.deltaTime);
            if (IsAtTarget()) SetState(State.Idle);
        }
        if (_state is State.Idle)
        {
            if (_currentTimer <= 0) SetState(State.Searching);
        }
        if (_state is State.Alert)
        {
            if (_currentTimer <= 0) SetState(State.Running);
        }
        if (_state is State.Running)
        {
            if (_followingPlayer)
            {
                if (Vector3.Distance(transform.position, PlayerController.Position) > _followDistance)
                {
                    _followingPlayer = false;
                    SetState(State.Searching);
                    return;
                }

                transform.position += (PlayerController.Position.ToVector3() - transform.position).normalized * _runningSpeed * GetSpeedModifier() * Time.deltaTime;
            }
            else
            {
                if (_creatureTarget == null || Vector3.Distance(transform.position, _creatureTarget.transform.position) > _followDistance)
                {
                    SetState(State.Searching);
                    return;
                }

                transform.position += (_creatureTarget.transform.position - transform.position).normalized * _runningSpeed * GetSpeedModifier() * Time.deltaTime;
            }

        }

        _currentTimer -= Time.deltaTime;
    }

    private void SetState(State state)
    {
        _state = state;

        if (_state is State.Searching)
        {
            if (_tileTarget != null) _tileTarget.IsTaken = false;

            _tileTarget = GetRandomTarget(Random.Range(2, 4));

            _tileTarget.IsTaken = true;

            _followingPlayer = false;
        }
        if (_state is State.Idle)
        {
            _currentTimer = Random.Range(.5f, 1f);

            AudioManager.Instance.Play(SoundEnum.HunterSearch, 0, false, GetStereoPan());

            if (_tileTarget != null) _tileTarget.IsTaken = true;
        }
        if (_state is State.Alert)
        {
            AudioManager.Instance.Play(SoundEnum.HunterFind, 0, false, GetStereoPan());
            _currentTimer = _alertTime;
        }
        if (_state is State.Running)
        {
            if (_tileTarget != null) _tileTarget.IsTaken = false;
        }

    }

    private bool IsAtTarget()
    {
        if (_tileTarget == null) return false;
        return Vector3.SqrMagnitude(_tileTarget.transform.position.SetZ(0) - transform.position) < .0001f;
    }

    private float GetSpeedModifier()
    {
        var tile = GridManager.Instance.GetTileTypeAt(transform.position);
        if (tile is TileType.TallGrass) return .8f;
        if (tile is TileType.Swamp) return .6f;
        return 1;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var creature = other.GetComponent<Creature>();
        if (creature == null) return;

        if (creature.Strength < Strength) creature.Die();
        else if (creature.Strength > Strength) Die();
    }

    private enum State
    {
        Searching,
        Idle,
        Alert,
        Running
    }
}
