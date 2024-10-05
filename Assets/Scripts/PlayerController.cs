using System;
using System.Collections;
using System.Collections.Generic;
using Foundation;
using UnityEngine;

public class PlayerController : Singleton<PlayerController>
{
    public static Vector2 Position => Instance.transform.position;

    [SerializeField] private float _movementSpeed = 1;
    [SerializeField] private LayerMask _obstaclesMask;
    [SerializeField] private float _strength;

    private Collider2D[] _colliders = new Collider2D[8];

    public bool IsActive;

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
        var tile = GridManager.Instance.GetTileTypeAt(transform.position);
        if (tile is TileType.Grass) movementSpeed *= .9f;
        if (tile is TileType.TallGrass) movementSpeed *= .8f;
        if (tile is TileType.Swamp) movementSpeed *= .8f;

        var movement = new Vector2(horizontalMovement, verticalMovement);

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

        if (creature.Strength < _strength) creature.Die();
    }
}