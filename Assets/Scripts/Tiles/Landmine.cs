using UnityEngine;
using UnityEngine.Tilemaps;

public class Landmine : MonoBehaviour
{
    [SerializeField] LandmineTile tileReference;
    void Start()
    {
        var animManager = GameObject.Find("TerrainManager").GetComponent<MineAnimationManager>();
        animManager.AddMine(transform.position);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Item"))
        {
            Destroy(collision.gameObject);
        }

        Destroy(gameObject);

        var tilemap = transform.parent.GetComponent<Tilemap>();
        var tilePosition = tilemap.WorldToCell(transform.position);

        tilemap.SetTile(tilePosition, null);
        SaveTerrain.RemoveTileSaveData(tilePosition, transform.parent.name);
    }

    void OnDestroy()
    {
        foreach (var applyDamage in Physics2D.OverlapCircleAll(transform.position, tileReference.ExplosionRadius))
        {
            if (!applyDamage.TryGetComponent<IDamageable>(out var damageable) &&
                !applyDamage.transform.parent.TryGetComponent(out damageable)) 
                continue;

            damageable.Damage(tileReference.Damage);
        }

        var terrainManager = GameObject.Find("TerrainManager");
        if(terrainManager && terrainManager.TryGetComponent<MineAnimationManager>(out var animManager))
        {
            animManager.DeleteMine(transform.position);
        }
    }
}
