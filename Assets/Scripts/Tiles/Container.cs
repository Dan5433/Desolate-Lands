using CustomClasses;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Container : InventoryBase, IBreakable
{
    [SerializeField] int guranteedEmptySlots;
    [SerializeField] GuranteedLootPool[] guranteedLootTable;
    [SerializeField] WeightedLootPool[] lootTable;

    private void Awake()
    {
        ui = ItemManager.Instance.ContainerUI.transform;
    }

    private void Start()
    {
        var tilemap = GetComponentInParent<Tilemap>();

        var container = tilemap.GetTile<ContainerTile>(
            tilemap.WorldToCell(transform.position)
        );

        guranteedEmptySlots = container.GuranteedEmptySlots;
        lootTable = container.LootTable;
        guranteedLootTable = container.GuranteedLootTable;
    }

    protected override string GetSaveKey()
    {
        return base.GetSaveKey() + transform.position;
    }

    public void GenerateInventory()
    {
        if (IsInventorySaved())
            return;

        if (lootTable.Length > 0 || guranteedLootTable.Length > 0)
            GenerateLoot();

        SaveInventory();
    }

    protected void GenerateLoot()
    {
        HashSet<int> availableSlots = new(inventory.Length);
        for (int i = 0; i < inventory.Length; i++)
            availableSlots.Add(i);

        foreach (var pool in guranteedLootTable)
        {
            for (int i = 0; i < pool.rolls; i++)
            {
                if (availableSlots.Count == 0)
                {
                    Debug.LogWarning("More guranteed loot than available inventory space!");
                    break;
                }

                InvItem item = WeightedUtils.RollItem(pool.loot);
                AddItemToRandomSlot(availableSlots, item);
            }
        }

        if (availableSlots.Count == 0)
        {
            Debug.LogWarning("No available inventory space for remaining loot table!");
            return;
        }

        int totalPoolWeight = 0;
        foreach (var pool in lootTable)
            totalPoolWeight += pool.weight;

        while (availableSlots.Count > guranteedEmptySlots)
        {
            InvItem item = WeightedUtils.RollItem(lootTable, totalPoolWeight);
            AddItemToRandomSlot(availableSlots, item);
        }

        foreach (int slot in availableSlots)
            inventory[slot] = ItemManager.Instance.InvItemAir;
    }

    void AddItemToRandomSlot(HashSet<int> availableSlots, InvItem itemToAdd)
    {
        int random = GameRandom.Range(0, availableSlots.Count);
        int slot = availableSlots.ToArray()[random];

        inventory[slot] = itemToAdd;
        availableSlots.Remove(slot);
    }

    public void OnBreak()
    {
        if (!IsInventorySaved())
            GenerateLoot();

        foreach (var item in inventory)
        {
            var dropItem = InventoryItemFactory.Create(
                item.ItemObj, item.Name, item.Count);

            ItemManager.SpawnGroundItem(dropItem, transform.position, new(0.5f, 0.5f));
        }

        DeleteInventory();
    }
}
