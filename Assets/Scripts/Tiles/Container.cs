using CustomClasses;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Container : InventoryBase, IBreakable
{
    private void Awake()
    {
        ui = ItemManager.Instance.ContainerUI.transform;
    }

    protected override string GetSaveKey()
    {
        return base.GetSaveKey() + transform.position;
    }

    public void GenInventory()
    {
        if (IsInventorySaved()) 
            return;

        var tilemap = GetComponentInParent<Tilemap>();

        GenLoot(tilemap.GetTile<ContainerTile>(
            tilemap.WorldToCell(transform.position)).LootPool);
        SaveInventory();
    }

    protected void GenLoot(WeightedItem[] lootTable)
    {
        int totalWeight = 0;
        foreach (var item in lootTable) totalWeight += item.weight;

        for (int i = 0; i < inventory.Length; i++)
        {
            int randomWeight = GameRandom.Range(0, totalWeight);
            foreach (var item in lootTable)
            {
                randomWeight -= item.weight;
                if (randomWeight < 0)
                {
                    var loot = InventoryItemFactory.Create(
                        item.item, item.item.Name, item.RandomCount());
                    inventory[i] = loot;
                    break;
                }
            }
        }
    }

    public void OnBreak()
    {
        var tilemap = GetComponentInParent<Tilemap>();

        if (!IsInventorySaved())
            GenLoot(tilemap.GetTile<ContainerTile>(
            tilemap.WorldToCell(transform.position)).LootPool);

        foreach (var item in inventory)
        {
            var dropItem = InventoryItemFactory.Create(
                item.ItemObj, item.Name, item.Count);

            ItemManager.SpawnGroundItem(dropItem, transform.position, new(0.5f,0.5f));
        }

        DeleteInventory();
    }
}
