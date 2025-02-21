using UnityEngine;
using UnityEngine.Tilemaps;

public class Landmine : MonoBehaviour
{
    [SerializeField] LandmineTile tileReference;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Item"))
            Destroy(collision.gameObject);

        Explode();

        var tilemap = transform.parent.GetComponent<Tilemap>();
        var tilePosition = tilemap.WorldToCell(transform.position);

        tilemap.SetTile(tilePosition, null);
        SaveTerrain.RemoveTileData(tilePosition, transform.parent.name);
    }

    void Explode()
    {
        float damage = Random.Range(tileReference.DamageRange.x, tileReference.DamageRange.y);

        foreach (var applyDamage in 
            Physics2D.OverlapCircleAll(transform.position, tileReference.ExplosionRadius))
        {
            if (!applyDamage.TryGetComponent<IDamageable>(out var damageable)) 
                continue;

            var direction = applyDamage.transform.position - transform.position;
            int mask = LayerMask.GetMask("Player", "Enemy", "Interact");
            float distanceTo = Vector2.Distance(transform.position, applyDamage.transform.position);

            RaycastHit2D matching = default;
            foreach(var hit in Physics2D.RaycastAll(transform.position, direction, distanceTo, mask))
            {
                matching = hit;
                if (matching.collider.gameObject.layer == applyDamage.gameObject.layer)
                    break;
            }
            
            if (!matching || matching.collider != applyDamage)
                continue;

            float distance = Vector2.Distance(transform.position, matching.point);
            float damageMagnitude = 1f - Mathf.Pow(distance / tileReference.ExplosionRadius, 2);

            float lerpedDamage = Mathf.Lerp(0, damage, damageMagnitude);
            if(lerpedDamage > 0)
                damageable.Damage(lerpedDamage);
        }

        MineAnimationManager.Instance.ExplodeMine(transform.position);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, tileReference.ExplosionRadius);
    }
}
