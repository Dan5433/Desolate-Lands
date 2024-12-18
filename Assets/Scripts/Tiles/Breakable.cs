using UnityEngine;
using UnityEngine.Tilemaps;

public class Breakable : MonoBehaviour, IDamageable
{
    [SerializeField] float health = 100f;

    private void Awake()
    {
        //TODO: save and load health
        UpdateSprite();
    }

    public void Damage(float damageAmount)
    {
        var tilemap = GetComponentInParent<Tilemap>();
        float hardness = tilemap.GetTile<BreakableTile>(
            tilemap.WorldToCell(transform.position)).Hardness;

        health -= damageAmount / hardness;
        UpdateSprite();

        if (health <= 0) OnBreak();
    }

    void UpdateSprite()
    {
        float percentBroken = (100f - health) / 100f;

        if (percentBroken == 0)
        {
            GetComponentInChildren<SpriteRenderer>().sprite = null;
            return;
        }

        for (int i = 0; i < BreakingManager.Instance.BreakSprites.Length; i++)
        {
            float highRange = (float)(i + 1) / BreakingManager.Instance.BreakSprites.Length;
            float lowRange = (float)i / BreakingManager.Instance.BreakSprites.Length;

            if (percentBroken >= lowRange && percentBroken <= highRange)
            {
                GetComponentInChildren<SpriteRenderer>().sprite = BreakingManager.Instance.BreakSprites[i];
                break;
            }
        }
    }

    public bool Heal(float healAmount)
    {
        health += healAmount;
        return true;
    }

    public void OnBreak()
    {
        foreach (var breakable in GetComponents<IBreakable>()) breakable.OnBreak();

        var tilemap = GetComponentInParent<Tilemap>();
        var tilePosition = tilemap.WorldToCell(transform.position);
        var tile = tilemap.GetTile<BreakableTile>(tilePosition);

        foreach (var drop in tile.Drops)
        {
            int count = drop.RandomCount();
            if (count == 0) { continue; }

            ItemManager.SpawnGroundItem(
                InventoryItemFactory.Create(drop.item, count), 
                transform.position, true);
        }

        tilemap.SetTile(tilePosition, null);
        //SaveTerrain.RemoveTileSaveData(breakCell, tilemap.name);
    }
}
