using CustomClasses;
using CustomExtensions;
using UnityEngine;

public class Container : InventoryBase
{
    [SerializeField] WeightedItem[] lootPool;
    static Vector3 offsetToCell = new(-0.5f, -0.5f, 0);

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

    void OnDestroy()
    {
        Debug.Log(GenerateTerrain.GetChunkIndexFromPosition(transform.position));
        if (!PlayerPrefs.HasKey(transform.parent.name + (transform.localPosition + offsetToCell).ToVector3Int()))
        {
            foreach (var item in inventory)
            {
                InvItem groundItem = new(item.ItemObj, item.Name, item.Count);

                ItemManager.SpawnGroundItem(groundItem, transform.position + offsetToCell, true);
            }
        }
    }
}
