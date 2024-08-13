using CustomClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
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

    async void Start()
    {
        inventory = new InvItem[invSize];

        for (int i = 0; i < inventory.Length; i++)
        {
            inventory[i] = ItemManager.Instance.InvItemAir;
        }

        var data = await LoadInventory();

        if (data != null)
        {
            InitInventory(data);
        }
        else
        {
            GenInventory();
            SaveInventory();
        }

        if (updateUIOnStart) UpdateUI();
    }

    public int AddToInventory(InvItem item)
    {
        int excess = item.Count;
        for (int i = 0; i < inventory.Length; i++)
        {
            if ((inventory[i].Count == inventory[i].ItemObj.MaxCount || inventory[i].ItemObj != item.ItemObj)
                && inventory[i].ItemObj != ItemManager.Instance.Air) continue;

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

    protected async Task<InventorySaveData> LoadInventory()
    {
        var fullPath = Path.Combine(GameManager.Instance.DataDirPath, "Storage");
        var dataHandler = new JsonFileDataHandler(fullPath, GetSaveKey());

        return await dataHandler.LoadDataAsync<InventorySaveData>();
    }

    protected virtual void InitInventory(InventorySaveData data)
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            var name = data.inventory[i].name;
            var count = data.inventory[i].count;
            var item = ItemManager.Instance.Items[data.inventory[i].id];
            inventory[i] = new InvItem(item, name, count);
        }
    }

    protected async void DeleteInventory()
    {
        var fullPath = Path.Combine(GameManager.Instance.DataDirPath, "Storage", GetSaveKey());

        await Task.Run(() => { File.Delete(Path.Combine(fullPath)); });
    }

    public async void SaveInventory()
    {
        var save = new InventorySaveData() { inventory = new(invSize) };

        for (int i = 0; i < inventory.Length; i++)
        {
            save.inventory.Add(
                new ItemSaveData
                {
                    id = inventory[i].ItemObj.Id,
                    name = inventory[i].Name,
                    count = inventory[i].Count
                });
        }

        var fullPath = Path.Combine(GameManager.Instance.DataDirPath, "Storage");
        var dataHandler = new JsonFileDataHandler(fullPath, GetSaveKey());

        await dataHandler.SaveDataAsync(save);
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

        int slotIndex = 0;
        for (int i = 0; i < ui.childCount; i++)
        {
            var slot = ui.GetChild(i);
            if (!ItemManager.IsInvSlot(slot.gameObject)) continue;

            ItemManager.UpdateItemUI(slot, inventory[slotIndex]);
            slotIndex++;
        }
    }

    protected virtual string GetSaveKey()
    {
        return gameObject.name;
    }
    protected virtual void GenInventory()
    {
    }

    public void SetUI(Transform ui)
    {
        this.ui = ui;
    }
}

[Serializable]
public class InventorySaveData
{
    public List<ItemSaveData> inventory = new();
}

[Serializable]
public class ItemSaveData
{
    public int id;
    public string name;
    public int count;
}