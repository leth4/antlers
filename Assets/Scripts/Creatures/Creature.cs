using System;
using System.Collections;
using System.Collections.Generic;
using Foundation;
using UnityEngine;

public class Creature : MonoBehaviour
{
    public static event Action<Creature> OnDied;

    [SerializeField] protected float Speed;
    [field: SerializeField] public float Strength { get; private set; }

    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Color _minColor;
    [SerializeField] private Color _maxColor;

    private Collider2D[] _colliders = new Collider2D[8];

    public void Initialize()
    {
        _spriteRenderer.color = Color.Lerp(_minColor, _maxColor, UnityEngine.Random.Range(0, 1f));
    }

    protected Transform GetNearbyPredator(float radius)
    {
        if (Vector3.Distance(transform.position, PlayerController.Instance.transform.position) < radius)
        {
            if (GridManager.Instance.GetTileTypeAt(PlayerController.Instance.transform.position) is not TileType.TallGrass) return PlayerController.Instance.transform;
        }

        var count = Physics2D.OverlapCircleNonAlloc(transform.position, radius, _colliders);
        for (int i = 0; i < count; i++)
        {
            var creature = _colliders[i]?.GetComponent<Creature>();
            if (creature == null || creature == this) continue;
            if (creature is HunterCreature && GridManager.Instance.GetTileTypeAt(creature.transform.position) is not TileType.TallGrass) return creature.transform;
        }

        return null;
    }

    protected bool CanHearPlayer(float radius)
    {
        if (Vector3.Distance(transform.position, PlayerController.Instance.transform.position) < radius)
        {
            if (GridManager.Instance.GetTileTypeAt(PlayerController.Instance.transform.position) is not TileType.TallGrass) return true;
        }
        return false;
    }

    protected Tile GetRandomTarget(float time)
    {
        for (int i = 0; i < 50; i++)
        {
            var direction = UnityEngine.Random.insideUnitCircle.normalized;
            var tile = GridManager.Instance.GetTileAt(transform.position + direction.ToVector3() * Speed * time);
            if (tile != null && !tile.IsTaken) return tile;
        }
        Debug.LogWarning("Can't find a target");
        return null;
    }

    protected Creature FindCreature(float radius, float maxStrength = 99)
    {
        var count = Physics2D.OverlapCircleNonAlloc(transform.position, radius, _colliders);

        for (int i = 0; i < count; i++)
        {
            var creature = _colliders[i]?.GetComponent<Creature>();
            if (creature != null && creature != this && creature.Strength < maxStrength) return creature;
        }

        return null;
    }

    protected void PlaySound(SoundEnum sound)
    {
        AudioManager.Instance.Play(sound, 0, false, GetStereoPan(), GetVolume());
    }

    public void Die(float delay = 0)
    {
        if (delay == 0)
        {
            OnDied?.Invoke(this);
            Destroy(gameObject);
        }
        else Tween.Delay(this, delay, () =>
        {
            OnDied?.Invoke(this);
            Destroy(gameObject);
        });
    }

    private float GetStereoPan()
    {
        if (GameManager.Instance.IsInfiniteMode) return 0;

        var pan = (transform.position.x - PlayerController.Instance.transform.position.x) / 5;

        if (pan < -1) pan = -.5f;
        if (pan > 1) pan = .5f;

        return pan;
    }

    private float GetVolume()
    {
        if (GameManager.Instance.IsInfiniteMode) return 1;
        return 1 - Mathf.Clamp(Vector3.Distance(PlayerController.Instance.transform.position, transform.position) / 15, 0, .8f);
    }
}
