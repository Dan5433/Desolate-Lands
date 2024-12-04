using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CraftStation : InventoryBase, IBreakable
{
    public CraftingStationType Type;
    public GameObject CraftingUi;

    void OnEnable()
    {
        ui = CraftingUi.transform.Find("Interactable");
    }

    protected override string GetSaveKey()
    {
        return base.GetSaveKey() + transform.position;
    }

    public void UpdateCraftingUI()
    {
        var title = CraftingUi.transform.Find("Title").GetComponent<TMP_Text>();

        title.text = name.Replace("(Clone)","");

        var craftingUi = CraftingUi.GetComponent<SlotCraftStationUI>();
        craftingUi.SetStation(this);
        craftingUi.LoadState(LoadProgress());
    }

    float LoadProgress()
    {
        string filePath = Path.Combine("Terrain", name + transform.position);
        BinaryDataHandler dataHandler = new(GameManager.DataDirPath, filePath);

        float progress = 0;
        if (!dataHandler.FileExists()) return progress;

        dataHandler.LoadData(reader =>
        {
            progress = reader.ReadSingle();
        });

        return progress;
    }

    public void SaveProgress(float progress)
    {
        string filePath = Path.Combine("Terrain", name + transform.position);
        BinaryDataHandler dataHandler = new(GameManager.DataDirPath, filePath);

        dataHandler.SaveData(writer =>
        {
            writer.Write(progress);
        });
    }

    public void OnBreak()
    {
        var tilePosition = transform.parent.GetComponent<Tilemap>().WorldToCell(transform.position);
        foreach (var item in inventory)
        {
            var dropItem = InventoryItemFactory.Create(
                item.ItemObj, item.Name, item.Count);

            ItemManager.SpawnGroundItem(dropItem, tilePosition, true);
        }

        DeleteInventory();
    }
}