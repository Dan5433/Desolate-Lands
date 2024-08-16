using System;
using System.IO;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class CraftStation : InventoryBase
{
    public StationType Type;
    public GameObject CraftingUi;

    void OnEnable()
    {
        ui = CraftingUi.transform.Find("Interactable");
    }

    protected override string GetSaveKey()
    {
        return base.GetSaveKey() + transform.position;
    }

    public async void UpdateCraftingUI()
    {
        var title = CraftingUi.transform.Find("Title").GetComponent<TMP_Text>();

        title.text = name.Replace("(Clone)","");

        var craftingUi = CraftingUi.GetComponent<SlotCraftStationUI>();
        craftingUi.SetStation(this);
        craftingUi.LoadState(await LoadProgress());
    }

    async Task<float> LoadProgress()
    {
        string filePath = Path.Combine("Terrain", name + transform.position);
        JsonFileDataHandler handler = new(GameManager.Instance.DataDirPath, filePath);

        var progress = await handler.LoadDataAsync<CraftStationSave>();

        if(progress == null) return 0;

        return progress.progress;
    }

    public async void SaveProgress(float progress)
    {
        CraftStationSave save = new() { progress = progress };

        string filePath = Path.Combine("Terrain", name + transform.position);
        JsonFileDataHandler handler = new(GameManager.Instance.DataDirPath, filePath);

        await handler.SaveDataAsync(save);
    }
}

[Serializable]
public class CraftStationSave
{
    public float progress;
}