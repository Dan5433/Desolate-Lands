using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;


public class PlayerCrafting : MonoBehaviour
{
    [SerializeField] ResourceUI[] resourcesUI;
    [SerializeField] PlayerResource[] resources;
    HashSet<CraftingRecipe> prototypedRecipes = new();
    const string saveString = "CraftingData";

    public PlayerResource[] Resources { get { return resources; } }
    public HashSet<CraftingRecipe> PrototypedRecipes {  get { return prototypedRecipes; } }

    void Start()
    {
        LoadCraftingData();
    }

    public void UnlockRecipe(CraftingRecipe recipe)
    {
        prototypedRecipes.Add(recipe);
    }

    public void AddResource(Resource type, int count)
    {
        int index = Array.FindIndex(resources, r => r.type == type);
        resources[index].count += count;

        UpdateUI();
    }

    void UpdateUI()
    {
        foreach(var resource in resources)
        {
            var UI = Array.Find(resourcesUI, ui => ui.type == resource.type).UI;

            UI.GetComponentInChildren<TMP_Text>().text = resource.count.ToString();

            if (!UI.gameObject.activeSelf && resource.count > 0) UI.gameObject.SetActive(true);
        }
    }

    async void LoadCraftingData()
    {
        string dirPath = Path.Combine(GameManager.Instance.DataDirPath, "Player");
        JsonFileDataHandler dataHandler = new(dirPath, saveString);

        var save = await dataHandler.LoadDataAsync<PlayerCraftingSave>();

        if (save != null)
        {
            resources = save.resources.ToArray();
            
            foreach(var index in save.prototypedRecipes)
            {
                prototypedRecipes.Add(CraftingManager.Instance.PlayerRecipes[index]);
            }
        }

        UpdateUI();
    }

    async void SaveCraftingData()
    {
        PlayerCraftingSave save = new();

        foreach(var resource in resources) save.resources.Add(resource);

        foreach (var recipe in prototypedRecipes)
        {
            int index = Array.FindIndex(CraftingManager.Instance.PlayerRecipes, r => r == recipe);
            save.prototypedRecipes.Add(index);
        }

        string dirPath = Path.Combine(GameManager.Instance.DataDirPath, "Player");
        JsonFileDataHandler dataHandler = new(dirPath, saveString);

        await dataHandler.SaveDataAsync(save);
    }

    private void OnDestroy()
    {
        SaveCraftingData();
    }
}

[Serializable]
public class PlayerCraftingSave
{
    public List<PlayerResource> resources = new();
    public List<int> prototypedRecipes = new();
}

[Serializable]
public struct PlayerResource
{
    public Resource type;
    public int count;
}

[Serializable]
public struct ResourceUI
{
    public Resource type;
    public Transform UI;
}

[Serializable]
public enum Resource
{
    Knowledge = 0,
}
