using CustomExtensions;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Landmine : MonoBehaviour
{
    [SerializeField] LandmineTile tileReference;
    void Start()
    {
        var animManager = GameObject.Find("TerrainManager").GetComponent<MineAnimationManager>();
        animManager.AddMine(transform.position.ToVector3Int());
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Item"))
        {
            Destroy(collision.gameObject);
        }

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

            float damageMagnitude = 
                (tileReference.ExplosionRadius - 
                Vector3.Distance(transform.position, applyDamage.transform.position)) /
                tileReference.ExplosionRadius;

            Debug.Log($"Damage magnitude for {applyDamage.gameObject.name}: {damageMagnitude}");

            damageable.Damage(Mathf.Lerp(0, tileReference.Damage, damageMagnitude));
        }

        var terrainManager = GameObject.Find("TerrainManager");
        if(terrainManager && terrainManager.TryGetComponent<MineAnimationManager>(out var animManager))
        {
            animManager.DeleteMine(transform.position.ToVector3Int());
        }
    }
}
