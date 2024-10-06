using System;
using System.Collections;
using System.Collections.Generic;
using Foundation;
using UnityEngine;

public class PlayerController : Singleton<PlayerController>
{
    [SerializeField] private float _movementSpeed = 1;
    [SerializeField] private LayerMask _obstaclesMask;
    [SerializeField] private float _strength;

    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private Sprite _defaultSprite;
    [SerializeField] private Sprite _walkSprite;

    private Collider2D[] _colliders = new Collider2D[8];

    public bool IsActive;
    public bool IsVisible;

    private bool _wasInTallGrass;

    private void Update()
    {
        if (!IsActive) return;
        HandleMovement();
    }

    private void HandleMovement()
    {
        var horizontalMovement = Input.GetAxisRaw("Horizontal");
        var verticalMovement = Input.GetAxisRaw("Vertical");

        var movementSpeed = _movementSpeed;

        var tile = GridManager.Instance.GetTileAt(transform.position);
        if (tile?.Type is TileType.Grass) movementSpeed *= .9f;
        if (tile?.Type is TileType.TallGrass)
        {
            if (!_wasInTallGrass)
            {
                StopAllCoroutines();
                StartCoroutine(WindVolumeRoutine(.65f));
            }
            _wasInTallGrass = true;
            movementSpeed *= .8f;
        }
        else if (_wasInTallGrass)
        {
            _wasInTallGrass = false;
            StopAllCoroutines();
            StartCoroutine(WindVolumeRoutine(1));
        }
        if (tile?.Type is TileType.Swamp) movementSpeed *= .8f;

        if (tile?.Type is TileType.Mine)
        {
            AudioManager.Instance.Play(SoundEnum.Mine);
            IsActive = false;
            GameManager.Instance.HandlePlayerDeath(DeathReason.Mine);
            tile.SetType(TileType.Normal, 0);
        }

        var movement = new Vector2(horizontalMovement, verticalMovement);

        if (horizontalMovement != 0 || verticalMovement != 0)
        {
            _renderer.sprite = Time.time % .5f > .25f ? _defaultSprite : _walkSprite;
            _renderer.flipX = horizontalMovement < 0;
        }
        else _renderer.sprite = _defaultSprite;

        if (!OverlapsAt(transform.position + new Vector2(horizontalMovement, verticalMovement).normalized.ToVector3() * movementSpeed * Time.deltaTime))
        {
            transform.position += new Vector2(horizontalMovement, verticalMovement).normalized.ToVector3() * movementSpeed * Time.deltaTime;
        }
        else if (!OverlapsAt(transform.position + new Vector2(horizontalMovement, 0).normalized.ToVector3() * movementSpeed * Time.deltaTime))
        {
            transform.position += new Vector2(horizontalMovement, 0).normalized.ToVector3() * movementSpeed * Time.deltaTime;
        }
        else if (!OverlapsAt(transform.position + new Vector2(0, verticalMovement).normalized.ToVector3() * movementSpeed * Time.deltaTime))
        {
            transform.position += new Vector2(0, verticalMovement).normalized.ToVector3() * movementSpeed * Time.deltaTime;
        }
    }

    private bool OverlapsAt(Vector2 position)
    {
        return Physics2D.OverlapCircleNonAlloc(position, transform.localScale.x / 2, _colliders, _obstaclesMask) != 0;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var creature = other.GetComponent<Creature>();
        if (creature == null) return;

        if (creature.Strength > _strength)
        {
            IsActive = false;
            if (creature is HunterCreature) GameManager.Instance.HandlePlayerDeath(DeathReason.Hunter);
            if (creature is SnakeCreature) GameManager.Instance.HandlePlayerDeath(DeathReason.Snake);
        }

        if (creature is DeerCreature) GameManager.Instance.HandleFoundFood();

        creature.Die();
    }

    private IEnumerator WindVolumeRoutine(float target)
    {
        var initial = AudioManager.Instance.GetChannelVolume(ChannelEnum.Ambient);
        for (float t = 0; t < .3f; t += Time.deltaTime)
        {
            AudioManager.Instance.SetChannelVolume(ChannelEnum.Ambient, Mathf.Lerp(initial, target, t / .3f));
            yield return null;
        }
    }
}