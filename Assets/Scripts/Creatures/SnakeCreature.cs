using System.Collections;
using System.Collections.Generic;
using Foundation;
using UnityEngine;

public class SnakeCreature : Creature
{
    [SerializeField] private float _alertRadius;
    [SerializeField] private float _alertTime;
    [SerializeField] private float _runningSpeed;
    [SerializeField] private float _followDistance;

    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private Sprite _defaultSprite;
    [SerializeField] private Sprite _walkSprite;

    private float _currentTimer;
    private Tile _tileTarget;
    private Transform _followTarget;

    private State _state;

    private void Start()
    {
        SetState(State.Searching);
    }

    private void Update()
    {
        if (_state != State.Running)
        {
            if (Vector3.Distance(transform.position, PlayerController.Instance.transform.position) < _alertRadius && GridManager.Instance.GetTileTypeAt(PlayerController.Instance.transform.position) == TileType.TallGrass)
            {
                SetState(State.Running);
                _followTarget = PlayerController.Instance.transform;
                return;
            }

            var creature = FindCreature(_alertRadius, 99);
            if (creature != null && GridManager.Instance.GetTileTypeAt(creature.transform.position) == TileType.TallGrass)
            {
                SetState(State.Running);
                _followTarget = creature.transform;
                return;
            }
        }

        if (_state is State.Searching)
        {
            transform.position = Vector3.MoveTowards(transform.position, _tileTarget.transform.position.SetZ(0), Speed * GetSpeedModifier() * Time.deltaTime);

            _renderer.sprite = Time.time % .3f > .15f ? _defaultSprite : _walkSprite;
            _renderer.flipX = transform.position.x > _tileTarget.transform.position.x;

            if (IsAtTarget()) SetState(State.Idle);
        }
        if (_state is State.Idle)
        {
            if (_currentTimer <= 0) SetState(State.Searching);
        }
        if (_state is State.Running)
        {
            if (_followTarget == null || Vector3.Distance(transform.position, _followTarget.position) > _followDistance || GridManager.Instance.GetTileTypeAt(_followTarget.position) != TileType.TallGrass)
            {
                SetState(State.Searching);
                return;
            }

            _renderer.sprite = Time.time % .2f > .1f ? _defaultSprite : _walkSprite;
            _renderer.flipX = transform.position.x > _followTarget.position.x;

            transform.position += (_followTarget.position - transform.position).normalized * _runningSpeed * GetSpeedModifier() * Time.deltaTime;
        }
        _currentTimer -= Time.deltaTime;
    }

    private void SetState(State state)
    {
        _state = state;

        if (_state is State.Searching)
        {
            if (_tileTarget != null) _tileTarget.IsTaken = false;

            var grassTile = GridManager.Instance.FindTile(transform.position, TileType.TallGrass);
            if (grassTile != null)
            {
                _tileTarget = grassTile;
                _tileTarget.IsTaken = true;
            }
            else SetState(State.Idle);

        }
        if (_state is State.Idle)
        {
            _currentTimer = Random.Range(1, 2);

            if (_tileTarget != null) _tileTarget.IsTaken = true;
        }
        else if (_state is State.Running)
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
        var tile = GridManager.Instance.GetTileAt(transform.position);
        if (tile?.Type is TileType.TallGrass) return 1;

        if (tile?.Type is TileType.Mine)
        {
            PlaySound(SoundEnum.Mine);
            Die();
            tile.SetType(TileType.Normal, 0);
        }

        return .5f;
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
        Running
    }
}
