using System.Collections;
using System.Collections.Generic;
using Foundation;
using UnityEngine;

public class Creature : MonoBehaviour
{
    [SerializeField] protected float Speed;

    [field: SerializeField] public float Strength { get; private set; }

    private Collider2D[] _colliders = new Collider2D[8];

    protected Vector2 GetRandomDirection(float time)
    {
        for (int i = 0; i < 20; i++)
        {
            var direction = Random.insideUnitCircle.normalized;
            if (GridManager.Instance.IsInBounds(transform.position + direction.ToVector3() * Speed * time * 1.5f)) return direction;
        }
        Debug.Log("Can't find direction");
        return Vector3.zero;
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
