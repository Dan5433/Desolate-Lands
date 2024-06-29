using CustomClasses;
using UnityEngine;
using Random = UnityEngine.Random;

public class InventoryBase : MonoBehaviour
{
    [SerializeField] protected InvItem[] inventory;
    [SerializeField] protected int invSize;
    [SerializeField] protected Transform ui;
    [SerializeField] protected bool updateUIOnStart = false;

    public Transform UI { get { return ui; } }
    public InvItem[] Inventory { get { return inventory; } }

    void Start()
    {
        inventory = new InvItem[invSize];

        Item air = ItemManager.Instance.Air;
        for (int i = 0; i < inventory.Length; i++)
        {
            inventory[i] = new InvItem(air, air.name, air.MaxCount);
        }

        if (PlayerPrefs.HasKey(GetSaveKey()))
        {
            LoadInventory();
        }
        else
        {
            GenInventory();
            SaveInventory();
        }

        if(updateUIOnStart) UpdateUI();
    }

    protected int AddToInventory(InvItem item)
    {
        int excess = item.Count;
        for (int i = 0; i < inventory.Length; i++)
        {
            if ((inventory[i].Count == inventory[i].ItemObj.MaxCount || inventory[i].ItemObj != item.ItemObj)
                && inventory[i].ItemObj != ItemManager.Instance.Air) { continue; }

            if (excess + inventory[i].Count <= item.ItemObj.MaxCount)
            {
                inventory[i] = new InvItem(item.ItemObj, item.Name, inventory[i].Count + excess);
                return 0;
            }
            else
            {
                excess -= item.ItemObj.MaxCount - inventory[i].Count;
                inventory[i] = new InvItem(item.ItemObj, item.Name, item.ItemObj.MaxCount);
                continue;
            }
        }
        return excess;
    }
    protected void GenLoot(WeightedItem[] lootTable)
    {

        int totalWeight = 0;
        foreach(var item in lootTable) totalWeight += item.Weight;

        for (int i = 0; i < inventory.Length; i++)
        {
            int randomWeight = Random.Range(0, totalWeight);
            foreach (var item in lootTable)
            {
                randomWeight -= item.Weight;
                if (randomWeight < 0)
                {
                    int count = Random.Range(1, item.Item.MaxCount);

                    InvItem loot = new(item.Item, item.Item.Name, count);
                    inventory[i] = loot;
                    break;
                }
            }
        }
    }

    protected virtual void LoadInventory()
    {
        for(int i = 0; i < inventory.Length; i++)
        {
            inventory[i] = new(ItemManager.Instance.Air, ItemManager.Instance.Air.Name, 0);
        }

        string[] items = PlayerPrefs.GetString(GetSaveKey()).Split(';');

        for (int i = 0; i < inventory.Length; i++)
        {
            string[] savedItem = items[i].Split(',');

            int id = int.Parse(savedItem[0]);
            string name = savedItem[1];
            int count = int.Parse(savedItem[2]);

            Item item = ItemManager.Instance.Items[id];
            inventory[i] = new InvItem(item, name, count);
        }
    }
    public void SaveInventory()
    {
        string data = "";

        foreach (var item in inventory)
        {
            data += $"{item.ItemObj.Id},{item.Name},{item.Count};";
        }

        PlayerPrefs.SetString(GetSaveKey(), data);
    }

    public void SetSlot(int index, InvItem item)
    {
        if (item == null || item.ItemObj == null) return;

        inventory[index] = item;
    }

    public void UpdateUI()
    {
        ui.GetComponent<InventoryRef>().Inventory = this;

        for (int i = ui.childCount; i < inventory.Length; i++)
        {
            Instantiate(ItemManager.Instance.InvSlot, ui);
        }

        for (int i = 0; i < ui.childCount; i++)
        {
            var slot = ui.GetChild(i);
            ItemManager.UpdateItemUI(slot, inventory[i]);
        }
    }

    protected virtual string GetSaveKey()
    {
        return gameObject.name;
    }
    protected virtual void GenInventory()
    {
    }
}
