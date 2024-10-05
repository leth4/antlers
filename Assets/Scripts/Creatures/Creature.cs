using System.Collections;
using System.Collections.Generic;
using Foundation;
using UnityEngine;

public class Creature : MonoBehaviour
{
    [SerializeField] protected float Speed;

    [field: SerializeField] public float Strength { get; private set; }

    private Collider2D[] _colliders = new Collider2D[8];

    protected Transform GetNearbyPredator(float radius)
    {
        if (Vector3.Distance(transform.position, PlayerController.Position) < radius)
        {
            if (GridManager.Instance.GetTileTypeAt(PlayerController.Position) is not TileType.TallGrass) return PlayerController.Instance.transform;
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
        if (Vector3.Distance(transform.position, PlayerController.Position) < radius)
        {
            if (GridManager.Instance.GetTileTypeAt(PlayerController.Position) is not TileType.TallGrass) return true;
        }
        return false;
    }

    protected Tile GetRandomTarget(float time)
    {
        for (int i = 0; i < 50; i++)
        {
            var direction = Random.insideUnitCircle.normalized;
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

    public void Die()
    {
        Destroy(gameObject);
    }

    protected float GetStereoPan()
    {
        var pan = (transform.position.x - PlayerController.Position.x) / 5;

        if (pan < -1) pan = -.3f;
        if (pan > 1) pan = .3f;

        return pan;
    }
}
