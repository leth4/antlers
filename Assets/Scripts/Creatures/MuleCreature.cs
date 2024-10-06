using System.Collections;
using System.Collections.Generic;
using Foundation;
using UnityEngine;

public class MuleCreature : Creature
{
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private Sprite _defaultSprite;
    [SerializeField] private Sprite _walkSprite;

    private State _state;

    private float _currentTimer;
    private Tile _currentTarget;

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
        else if (_state is State.Idle)
        {
            HandleIdle();
        }

        _currentTimer -= Time.deltaTime;
    }

    private void HandleSearch()
    {
        transform.position = Vector3.MoveTowards(transform.position, _currentTarget.transform.position.SetZ(0), Speed * GetSpeedModifier() * Time.deltaTime);

        _renderer.sprite = Time.time % .6f > .3f ? _defaultSprite : _walkSprite;
        _renderer.flipX = transform.position.x > _currentTarget.transform.position.x;

        if (IsAtTarget()) SetState(State.Idle);
    }

    private void HandleIdle()
    {
        if (_currentTimer <= 0) SetState(State.Searching);
    }

    private void SetState(State state)
    {
        _state = state;

        if (_state is State.Searching)
        {
            _renderer.sprite = _defaultSprite;

            if (_currentTarget != null) _currentTarget.IsTaken = false;

            _currentTarget = GetRandomTarget(Random.Range(5, 8));
            if (_currentTarget == null)
            {
                Die();
                return;
            }

            _currentTarget.IsTaken = true;
        }
        if (_state is State.Idle)
        {
            PlaySound(SoundEnum.Mule);

            _renderer.sprite = _defaultSprite;

            _currentTimer = Random.Range(1, 2);

            if (_currentTarget != null) _currentTarget.IsTaken = true;
        }
    }

    private bool IsAtTarget()
    {
        if (_currentTarget == null) return false;
        return Vector3.SqrMagnitude(_currentTarget.transform.position.SetZ(0) - transform.position) < .0001f;
    }

    public override void Die(float delay = 0)
    {
        base.Die(0);

        PlaySound(SoundEnum.MuleDie);

        GridManager.Instance.TurnCircleNeighborsInto(transform.position, TileType.TallGrass);
    }

    private float GetSpeedModifier()
    {
        var tile = GridManager.Instance.GetTileAt(transform.position);

        if (tile.Type != TileType.Normal && tile.Type != TileType.Mine)
        {
            tile.SetType(TileType.Normal, 0);
        }

        if (tile?.Type is TileType.Mine)
        {
            PlaySound(SoundEnum.Mine);
            Die();
            tile.SetType(TileType.Normal, 0);
        }

        return 1;
    }

    private enum State
    {
        Searching,
        Idle
    }
}
