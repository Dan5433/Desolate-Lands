using CustomClasses;
using System.Collections.Generic;
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
            tilemap.WorldToCell(transform.position)).LootTable);

        SaveInventory();
    }

    protected void GenLoot(WeightedLootPool[] lootTable)
    {
        HashSet<int> slots = new(inventory.Length);
        for (int i = 0; i < inventory.Length; i++)
            slots.Add(i);

        int totalPoolWeight = 0;
        foreach (var pool in lootTable)
            totalPoolWeight += pool.weight;

        for (int i = 0; i < inventory.Length; i++)
            inventory[i] = WeightedUtils.RollItem(lootTable, totalPoolWeight);
    }

    public void OnBreak()
    {
        var tilemap = GetComponentInParent<Tilemap>();

        if (!IsInventorySaved())
            GenLoot(tilemap.GetTile<ContainerTile>(
            tilemap.WorldToCell(transform.position)).LootTable);

        foreach (var item in inventory)
        {
            var dropItem = InventoryItemFactory.Create(
                item.ItemObj, item.Name, item.Count);

            ItemManager.SpawnGroundItem(dropItem, transform.position, new(0.5f,0.5f));
        }

        DeleteInventory();
    }
}
