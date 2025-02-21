using UnityEngine;

public class LandmineTile : AnimatedGameobjectTile
{
    [SerializeField] float explosionRadius;
    [SerializeField] Vector2 damageRange;

    public float ExplosionRadius => explosionRadius;
    public Vector2 DamageRange => damageRange;
}
