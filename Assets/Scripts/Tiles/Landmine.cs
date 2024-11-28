using UnityEngine;
using UnityEngine.Tilemaps;

public class Landmine : MonoBehaviour
{
    void Start()
    {
        var animManager = GameObject.Find("TerrainManager").GetComponent<MineAnimationManager>();
        animManager.AddMine(transform);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            Destroy(gameObject);
        }

        if (collision.transform.CompareTag("Item"))
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }

        if(collision.TryGetComponent<LivingBase>(out var living) || 
            collision.transform.parent.TryGetComponent(out living))
        {
            living.Damage(50);
        }

        var tilemap = transform.parent.GetComponent<Tilemap>();
        var tilePosition = tilemap.WorldToCell(transform.position);

        tilemap.SetTile(tilePosition, null);
        SaveTerrain.RemoveTileSaveData(tilePosition, transform.parent.name);
    }

    void OnDestroy()
    {
        var terrainManager = GameObject.Find("TerrainManager");
        if(terrainManager && terrainManager.TryGetComponent<MineAnimationManager>(out var animManager))
        {
            animManager.DeleteMine(transform);
        }
    }
}
