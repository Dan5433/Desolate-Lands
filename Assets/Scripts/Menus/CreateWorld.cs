using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class CreateWorld : MonoBehaviour
{
    [SerializeField] TMP_InputField worldName;
    [SerializeField] TMP_InputField worldSeed;

    public void Create()
    {
        string name = ParseName();

        if (int.TryParse(worldSeed.text,out int seed))
        {
            Debug.Log("Seed succesfully parsed: "+seed);
            PlayerPrefs.SetInt(name, seed);
        }
        else
        {
            Debug.Log("Initializing new seed for world: " + worldName);
            PlayerPrefs.SetInt(name, Random.Range(int.MinValue, int.MaxValue));
        }

        CreateStatsFile(name);

        GameManager.PendingWorldName = name;
        SceneManager.LoadScene("Game");
    }

    void CreateStatsFile(string worldName)
    {
        var date = DateTime.Now;
        WorldStats stats = new(){ 
            creationDate = new(date.Year, date.Month, date.Day) 
        };

        var worldPath = Path.Combine(MainMenuManager.SavesDirPath,worldName);

        BinaryDataHandler handler = new(worldPath, MainMenuManager.StatsFileName);
        handler.SaveData(writer => stats.Write(writer));
    }

    string ParseName()
    {
        string name = worldName.text != string.Empty ? worldName.text : "New World";

        int existingCounter = 0;
        foreach (var dir in Directory.GetDirectories(MainMenuManager.SavesDirPath, $"*{name}*"))
            existingCounter++;

        Debug.Log($"Found {existingCounter} existing worlds with same name");

        if (existingCounter > 0)
            name += $" ({existingCounter})";

        return name;
    }
}