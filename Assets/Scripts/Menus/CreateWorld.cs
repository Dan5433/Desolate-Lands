using System;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class CreateWorld : MonoBehaviour
{
    [SerializeField] TMP_InputField worldNameInput;
    [SerializeField] TMP_InputField worldSeedInput;

    public void Create()
    {
        string name = ParseName();
        ParseSeed(name);

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
        string name = !string.IsNullOrWhiteSpace(worldNameInput.text) 
            ? worldNameInput.text : "New World";

        string modifiedName = name;
        int existingCounter = 0;

        var existingWorlds = Directory.GetDirectories(MainMenuManager.SavesDirPath)
                              .Select(Path.GetFileName)
                              .ToHashSet();
        
        while (existingWorlds.Contains(modifiedName))
        {
            existingCounter++;
            modifiedName = $"{name} ({existingCounter})";
        }

        Debug.Log($"Found {existingCounter} existing worlds with same name");

        return modifiedName;
    }

    void ParseSeed(string worldName)
    {
        RandomStateWrapper randomStateWrapper;

        if (worldSeedInput.text != string.Empty)
        {
            Debug.Log("Seed succesfully parsed for world: " + worldName);

            var hash = Hash128.Compute(worldSeedInput.text);
            randomStateWrapper = new(hash);
        }
        else
        {
            Debug.Log("Initializing new seed for world: " + worldName);

            randomStateWrapper = new(
                Random.Range(int.MinValue,int.MaxValue), Random.Range(int.MinValue, int.MaxValue),
                Random.Range(int.MinValue, int.MaxValue), Random.Range(int.MinValue, int.MaxValue));
        }

        GameRandom.Init(randomStateWrapper);
    }
}