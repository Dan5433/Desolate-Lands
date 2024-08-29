using CustomClasses;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Container : InventoryBase, IBreakable
{
    [SerializeField] WeightedItem[] lootPool;

    public WeightedItem[] LootPool { set { lootPool = value; } }
    public int InvSize { set { invSize = value; } }

    void OnEnable()
    {
        ui = ItemManager.Instance.ContainerUI.transform;
    }

    protected override string GetSaveKey()
    {
        return base.GetSaveKey() + transform.position;
    }

    public void GenInventory()
    {
        if (IsInventorySaved()) return;

        GenLoot(lootPool);
        SaveInventory();
    }

    protected void GenLoot(WeightedItem[] lootTable)
    {
        int totalWeight = 0;
        foreach (var item in lootTable) totalWeight += item.weight;

        for (int i = 0; i < inventory.Length; i++)
        {
            int randomWeight = Random.Range(0, totalWeight);
            foreach (var item in lootTable)
            {
                randomWeight -= item.weight;
                if (randomWeight < 0)
                {
                    int count = Random.Range(1, item.item.MaxCount);

                    InvItem loot = new(item.item, item.item.Name, count);
                    inventory[i] = loot;
                    break;
                }
            }
        }
    }

    public void OnBreak()
    {
        if (!IsInventorySaved()) GenLoot(lootPool);

        var tilePosition = transform.parent.GetComponent<Tilemap>().WorldToCell(transform.position);
        foreach (var item in inventory)
        {
            ItemManager.SpawnGroundItem(item, tilePosition, true);
        }

        DeleteInventory();
    }
}
