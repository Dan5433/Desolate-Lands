using System;
using System.IO;
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
        string name = worldNameInput.text != string.Empty ? worldNameInput.text : "New World";

        int existingCounter = 0;
        foreach (var dir in Directory.GetDirectories(MainMenuManager.SavesDirPath, $"*{name}*"))
            existingCounter++;

        Debug.Log($"Found {existingCounter} existing worlds with same name");

        if (existingCounter > 0)
            name += $" ({existingCounter})";

        return name;
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