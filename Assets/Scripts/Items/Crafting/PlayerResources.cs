using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;


public class PlayerResources : MonoBehaviour
{
    [SerializeField] ResourceUI[] resourcesUI;
    [SerializeField] PlayerResource[] resources;

    void Start()
    {
        LoadResourceData();
    }

    public void AddResource(Resources type, int count)
    {
        int index = Array.FindIndex(resources, r => r.resource == type);
        resources[index].count += count;

        UpdateUI();
    }

    void UpdateUI()
    {
        foreach(var resource in resources)
        {
            var UI = Array.Find(resourcesUI, ui => ui.type == resource.resource).UI;

            UI.GetComponentInChildren<TMP_Text>().text = resource.count.ToString();

            if (!UI.gameObject.activeSelf && resource.count > 0) UI.gameObject.SetActive(true);
        }
    }

    async void LoadResourceData()
    {
        string dirPath = Path.Combine(GameManager.Instance.DataDirPath, "Player");
        JsonFileDataHandler dataHandler = new(dirPath, "ResourceData");

        var save = await dataHandler.LoadDataAsync<PlayerCraftingSave>();

        if (save != null) resources = save.resources.ToArray();

        UpdateUI();
    }

    async void SaveResourceData()
    {
        PlayerCraftingSave save = new();

        foreach(var resource in resources)
        {
            save.resources.Add(resource);
        }

        string dirPath = Path.Combine(GameManager.Instance.DataDirPath, "Player");
        JsonFileDataHandler dataHandler = new(dirPath, "ResourceData");

        await dataHandler.SaveDataAsync(save);
    }

    private void OnDestroy()
    {
        SaveResourceData();
    }
}

[Serializable]
public class PlayerCraftingSave
{
    public List<PlayerResource> resources = new();

}

[Serializable]
public struct PlayerResource
{
    public Resources resource;
    public int count;
}

[Serializable]
public struct ResourceUI
{
    public Resources type;
    public Transform UI;
}

[Serializable]
public enum Resources
{
    Gears = 0,
}
