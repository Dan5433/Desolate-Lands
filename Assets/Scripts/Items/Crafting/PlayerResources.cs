using System;
using System.IO;
using TMPro;
using UnityEngine;


public class PlayerResources : MonoBehaviour
{
    [SerializeField] Transform resources;
    int availableGears;

    void Start()
    {
        LoadResourceData();
    }

    public void AddGears(int gears)
    {
        availableGears += gears;

        UpdateUI();
    }

    void UpdateUI()
    {
        var gears = resources.transform.Find("Gears");
        gears.GetComponentInChildren<TMP_Text>().text = availableGears.ToString();

        if(!gears.gameObject.activeSelf && availableGears > 0) gears.gameObject.SetActive(true);
    }

    async void LoadResourceData()
    {
        string dirPath = Path.Combine(GameManager.Instance.DataDirPath, "Player");
        JsonFileDataHandler dataHandler = new(dirPath, "ResourceData");

        var save = await dataHandler.LoadDataAsync<PlayerCraftingSave>();

        if (save != null)
        {
            availableGears = save.gears;
        }
        else
        {
            availableGears = 0;
        }

        UpdateUI();
    }

    async void SaveResourceData()
    {
        PlayerCraftingSave save = new()
        {
            gears = availableGears,
        };

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
    public int gears;
}
