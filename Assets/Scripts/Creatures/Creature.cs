using System.Collections;
using System.Collections.Generic;
using Foundation;
using UnityEngine;

public class Creature : MonoBehaviour
{
    [SerializeField] protected float Speed;

    [field: SerializeField] public float Strength { get; private set; }

    private Collider2D[] _colliders = new Collider2D[8];

    protected bool CanHearPlayer(float radius)
    {
        if (Vector3.Distance(transform.position, PlayerController.Position) > radius) return false;
        if (GridManager.Instance.GetTileTypeAt(PlayerController.Position) is TileType.TallGrass) return false;

        return true;
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

    protected Creature FindCreature(Vector2 position, float radius, float maxStrength = 99)
    {
        if (Physics2D.OverlapCircleNonAlloc(position, radius, _colliders) != 0)
        {
            foreach (var collider in _colliders)
            {
                var creature = collider?.GetComponent<Creature>();
                if (creature != null && creature.Strength < maxStrength) return creature;
            }
        }

        return null;
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
