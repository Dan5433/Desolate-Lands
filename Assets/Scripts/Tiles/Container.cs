using CustomClasses;
using CustomExtensions;
using TreeEditor;
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

    protected override void GenInventory()
    {
        GenLoot(lootPool);
    }

    public void OnBreak()
    {
        var tilePosition = transform.parent.GetComponent<Tilemap>().WorldToCell(transform.position);
        foreach (var item in inventory)
        {
            ItemManager.SpawnGroundItem(item, tilePosition, true);
        }

        DeleteInventory();
    }
}
