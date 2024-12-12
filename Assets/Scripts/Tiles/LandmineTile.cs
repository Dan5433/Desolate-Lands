using UnityEngine;

public class LandmineTile : AnimatedGameobjectTile
{
    [SerializeField] float explosionRadius;
    [SerializeField] float damage;

    public float ExplosionRadius => explosionRadius;
    public float Damage => damage;
}
