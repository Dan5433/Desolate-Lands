using CustomExtensions;
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
        foreach (var applyDamage in Physics2D.OverlapCircleAll(
            transform.position, tileReference.ExplosionRadius))
        {
            if (!applyDamage.TryGetComponent<IDamageable>(out var damageable)) 
                continue;

            //TODO: add raycast going to each object to get accurate distance
            //var direction = applyDamage.transform.position - transform.position;
            //int mask = LayerMask.GetMask("Player", "Enemy", "Interact");
            //var raycast = Physics2D.Raycast(transform.position, direction,);

            float damageMagnitude = 
                (tileReference.ExplosionRadius - 
                Vector3.Distance(transform.position, applyDamage.transform.position)) /
                tileReference.ExplosionRadius;

            Debug.Log($"Damage magnitude for {applyDamage.gameObject.name}: {damageMagnitude}");

            damageable.Damage(Mathf.Lerp(0, tileReference.Damage, damageMagnitude));
        }

        MineAnimationManager.Instance.ExplodeMine(transform.position);
    }
}
