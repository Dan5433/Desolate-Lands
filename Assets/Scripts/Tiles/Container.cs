using CustomClasses;
using EditorAttributes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Container : InventoryBase, IBreakable
{
    [SerializeField] int guaranteedEmptySlots;
    [SerializeField] GuaranteedLootPool[] guaranteedLootTable;
    [SerializeField] WeightedLootPool[] lootTable;

    protected override void Awake()
    {
        base.Awake();

        ui = ItemManager.Instance.ContainerUI.transform;
    }

    protected override void Start()
    {
        base.Start();

        var tilemap = GetComponentInParent<Tilemap>();

        var container = tilemap.GetTile<ContainerTile>(
            tilemap.WorldToCell(transform.position)
        );

        guaranteedEmptySlots = container.GuaranteedEmptySlots;
        lootTable = container.LootTable;
        guaranteedLootTable = container.GuaranteedLootTable;
    }

    protected override string GetSaveKey()
    {
        return base.GetSaveKey() + transform.position;
    }

    [Button(buttonHeight: 36)]
    public void GenerateInventory()
    {
        if (IsInventorySaved())
            return;

        if (lootTable.Length > 0 || guaranteedLootTable.Length > 0)
            GenerateLoot();

        SaveInventory();
    }

    protected void GenerateLoot()
    {
        HashSet<int> availableSlots = new(inventory.Length);
        for (int i = 0; i < inventory.Length; i++)
            availableSlots.Add(i);

        foreach (var pool in guaranteedLootTable)
        {
            for (int i = 0; i < pool.rolls; i++)
            {
                if (availableSlots.Count == 0)
                {
                    Debug.LogWarning("More guaranteed loot than available inventory space!");
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

        while (availableSlots.Count > guaranteedEmptySlots)
        {
            InvItem item = WeightedUtils.RollItem(lootTable, totalPoolWeight);
            AddItemToRandomSlot(availableSlots, item);
        }

        foreach (int slot in availableSlots)
            inventory[slot] = ItemManager.Instance.InvItemAir;
    }

    void AddItemToRandomSlot(HashSet<int> availableSlots, InvItem itemToAdd)
    {
        SeededRandom generator = RNGManager.Instance.Generators.loot;
        int random = generator.Range(0, availableSlots.Count);
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
