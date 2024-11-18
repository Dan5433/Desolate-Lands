using CustomClasses;
using EditorAttributes;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class InventoryBase : MonoBehaviour
{
    [SerializeField] protected InvItem[] inventory;
    [SerializeField] protected Transform ui;
    [SerializeField] protected bool updateUIOnStart = false;

    public Transform UI => ui;
    public InvItem[] Inventory => inventory;

    void Start()
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            inventory[i] = ItemManager.Instance.InvItemAir;
        }

        if (IsInventorySaved()) LoadInventory();

        if (updateUIOnStart) UpdateUI();
        InitInventory();
    }

    [Button("Add To Inventory", 30)]
    public void AdminAdd(int itemIndex, int count)
    {
        var item = ItemManager.Instance.Items[itemIndex];
        AddToInventory(InventoryItemFactory.Create(item,item.name,count));
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
                inventory[i] = InventoryItemFactory.Create(
                    item.ItemObj, item.Name, inventory[i].Count + excess);
                return 0;
            }
            else
            {
                excess -= item.ItemObj.MaxCount - inventory[i].Count;
                inventory[i] = InventoryItemFactory.Create(
                    item.ItemObj, item.Name, item.ItemObj.MaxCount);
                continue;
            }
        }

        return excess;
    }

    protected bool IsInventorySaved()
    {
        var fullPath = Path.Combine(GameManager.DataDirPath, "Storage", GetSaveKey());
        return File.Exists(fullPath);
    }

    protected virtual void InitInventory() { }

    void LoadInventory()
    {
        var dirPath = Path.Combine(GameManager.DataDirPath, "Storage");
        var dataHandler = new BinaryDataHandler(dirPath, GetSaveKey());

        dataHandler.LoadData(reader =>
        {
            for(int i = 0; i < inventory.Length; i++)
            {
                inventory[i] = InventoryItemFactory.Create(reader);
            }
        });
    }

    protected async void DeleteInventory()
    {
        var fullPath = Path.Combine(GameManager.DataDirPath, "Storage", GetSaveKey());

        await Task.Run(() => { File.Delete(Path.Combine(fullPath)); });
    }

    public void SaveInventory()
    {
        var dirPath = Path.Combine(GameManager.DataDirPath, "Storage");
        var dataHandler = new BinaryDataHandler(dirPath, GetSaveKey());

        dataHandler.SaveData(writer =>
        {
            foreach (var item in inventory) item.Save(writer);
        });
    }

    public void SetSlot(int index, InvItem item)
    {
        if (item == null || item.ItemObj == null) return;

        inventory[index] = item;
    }

    public virtual void UpdateUI()
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

    public void SetUI(Transform ui)
    {
        this.ui = ui;
    }
}