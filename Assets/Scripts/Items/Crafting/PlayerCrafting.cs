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
    [SerializeField] PlayerCraftingUI ui;
    [SerializeField] Interact interact;
    [SerializeField] PlayerInventory inventory;
    HashSet<CraftingRecipe> prototypedRecipes = new();
    const string saveString = "CraftingData";

    public PlayerResource[] Resources => resources;
    public HashSet<CraftingRecipe> PrototypedRecipes => prototypedRecipes;

    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.E)) return;

        if (!ui.gameObject.activeSelf)
        {
            ui.UpdateAvailableCraftingUI(inventory.Inventory);
            interact.DisableUI();
            interact.UpdateUI(gameObject, ui.gameObject);
            ui.gameObject.SetActive(true);
        }
        else interact.DisableUI();
    }

    void Start()
    {
        LoadCraftingData();
    }

    public void UnlockRecipe(CraftingRecipe recipe)
    {
        prototypedRecipes.Add(recipe);
    }

    public void ChangeResourceCount(Resource type, int changeBy)
    {
        int index = Array.FindIndex(resources, r => r.type == type);
        resources[index].count += changeBy;

        UpdateUI();
    }

    void UpdateUI()
    {
        foreach(var resource in resources)
        {
            var UI = Array.Find(resourcesUI, ui => ui.type == resource.type).UI;

            UI.GetComponentInChildren<TMP_Text>().text = resource.count.ToString();
        }
    }

    void LoadCraftingData()
    {
        string dirPath = Path.Combine(GameManager.DataDirPath, "Player");   
        BinaryDataHandler dataHandler = new(dirPath, saveString);

        if (!dataHandler.FileExists()) return;

        dataHandler.LoadData(reader =>
        {
            for (int i = 0; i < resources.Length; i++)
            {
                resources[i].count = reader.ReadInt32();
            }

            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                prototypedRecipes.Add(
                    CraftingManager.Instance.CraftingRecipes[reader.ReadInt32()]);
            }
        });

        UpdateUI();
    }

    void SaveCraftingData()
    {
        string dirPath = Path.Combine(GameManager.DataDirPath, "Player");
        BinaryDataHandler dataHandler = new(dirPath, saveString);

        dataHandler.SaveData(writer =>
        {
            foreach(var resource in resources)
            {
                writer.Write(resource.count);
            }

            writer.Write(prototypedRecipes.Count);
            foreach(var recipe in prototypedRecipes)
            {
                int index = Array.FindIndex(
                    CraftingManager.Instance.CraftingRecipes, r => r == recipe);

                writer.Write(index);
            }
        });
    }

    private void OnApplicationQuit()
    {
        SaveCraftingData();
    }
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
