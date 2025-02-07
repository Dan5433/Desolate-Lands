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
    static string dataDirPath;

    public Transform UI => ui;
    public InvItem[] Inventory => inventory;

    private void Awake()
    {
        dataDirPath = Path.Combine(GameManager.DataDirPath, "storage");
    }

    void Start()
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            inventory[i] = ItemManager.Instance.InvItemAir;
        }

        LoadInventory();

        if (updateUIOnStart) 
            UpdateUI();

        InitInventory();
    }

    [Button("Add To Inventory", 30)]
    public void AdminAdd(int itemIndex, int count)
    {
        var item = ItemManager.Instance.Items[itemIndex];
        AddToInventory(InventoryItemFactory.Create(item,count));
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
                item.Count = inventory[i].Count + excess;
                inventory[i] = item;
                return 0;
            }
            else
            {
                excess -= item.ItemObj.MaxCount - inventory[i].Count;
                inventory[i] = item;
                continue;
            }
        }

        ItemManager.SpawnGroundItem(
            InventoryItemFactory.Create(item.ItemObj,excess), 
            gameObject.transform.position);

        return excess;
    }

    protected bool IsInventorySaved()
    {
        var fullPath = Path.Combine(dataDirPath, GetSaveKey());
        return File.Exists(fullPath);
    }

    protected virtual void InitInventory() { }

    void LoadInventory()
    {
        var dataHandler = new BinaryDataHandler(dataDirPath, GetSaveKey());

        if (!dataHandler.FileExists())
            return;

        dataHandler.LoadData(reader =>
        {
            for(int i = 0; i < inventory.Length; i++)
                inventory[i] = InventoryItemFactory.Create(reader);
        });
    }

    protected async void DeleteInventory()
    {
        var fullPath = Path.Combine(dataDirPath, GetSaveKey());
        if (!File.Exists(fullPath))
            return;

        await Task.Run(() => { File.Delete(fullPath); });
    }

    public void SaveInventory()
    {
        var dataHandler = new BinaryDataHandler(dataDirPath, GetSaveKey());

        dataHandler.SaveData(writer =>
        {
            foreach (var item in inventory) 
                item.Save(writer);
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
            if (!ItemManager.IsInvSlot(slot.gameObject)) 
                continue;

            ItemManager.UpdateItemUI(slot, inventory[slotIndex]);
            slotIndex++;
        }
    }

    public virtual void ClearInventory()
    {
        for (int i = 0; i < inventory.Length; i++)
            inventory[i] = ItemManager.Instance.InvItemAir;

        UpdateUI();
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