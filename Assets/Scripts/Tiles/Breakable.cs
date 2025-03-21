using CustomExtensions;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Breakable : MonoBehaviour, IDamageable
{
    [SerializeField] float health = 100f;

    private void Start()
    {
        //TODO alpha 1.0.1: save and load health
        UpdateSprite();
    }

    public bool Damage(float damageAmount)
    {
        var tilemap = GetComponentInParent<Tilemap>();
        float hardness = tilemap.GetTile<BreakableTile>(
            tilemap.WorldToCell(transform.position)).Hardness;

        health -= damageAmount / hardness;
        UpdateSprite();

        if (health <= 0) 
            OnBreak();

        return true;
    }

    void UpdateSprite()
    {
        float percentBroken = (100f - health) / 100f;
        var position = GetComponentInParent<Tilemap>().WorldToCell(transform.position);

        BreakingManager.UpdateBreakStage(position, percentBroken);
    }

    public bool Heal(float healAmount)
    {
        health += healAmount;
        return true;
    }

    public void OnBreak()
    {
        foreach (var breakable in GetComponents<IBreakable>()) 
            breakable.OnBreak();

        var tilemap = GetComponentInParent<Tilemap>();
        var tilePosition = tilemap.WorldToCell(transform.position);
        var tile = tilemap.GetTile<BreakableTile>(tilePosition);

        BreakingManager.UpdateBreakStage(tilePosition, 0);

        foreach (var drop in tile.Drops)
            ItemManager.SpawnGroundItem(drop.Roll(), new(0.5f, 0.5f));

        foreach (var particle in BreakingManager.Instance.BreakParticles)
        {
            var instance = particle.PlayOneShot(transform.position,
                Quaternion.Euler(0, 0, 90f));

            instance.ChangeColors(tile.Colors[0], tile.Colors[1]);
        }

        tilemap.SetTile(tilePosition, null);
        SaveTerrain.RemoveTileData(tilePosition, tilemap.name);
    }
}
