using System;
using System.IO;
using System.Linq;
using System.Text;
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
        WorldStats stats = new()
        {
            creationDate = new(date.Year, date.Month, date.Day)
        };

        var worldPath = Path.Combine(MainMenuManager.SavesDirPath, worldName);

        BinaryDataHandler handler = new(worldPath, MainMenuManager.StatsFileName);
        handler.SaveData(writer => stats.Write(writer));
    }

    string ParseName()
    {
        string name = !string.IsNullOrWhiteSpace(worldNameInput.text)
            ? worldNameInput.text
            : "New World";

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
        string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()_+-=[]{};':\",.<>?/\\|`~";

        Hash128 hash;

        if (!string.IsNullOrWhiteSpace(worldSeedInput.text))
        {
            Debug.Log("Seed succesfully parsed for world: " + worldName);

            Debug.Log("Seed: " + worldSeedInput.text);
            hash = Hash128.Compute(worldSeedInput.text);
        }
        else
        {
            Debug.Log("Initializing random seed for world: " + worldName);

            int length = Random.Range(1, worldSeedInput.characterLimit + 1 /* account for exclusive max */ );

            StringBuilder seed = new(length);

            for (int i = 0; i < length; i++)
            {
                int charIndex = Random.Range(0, chars.Length);
                seed.Append(chars[charIndex]);
            }

            Debug.Log("Seed: " + seed.ToString());
            hash = Hash128.Compute(seed.ToString());
        }

        RandomStateWrapper randomStateWrapper = new(hash);

        GameRandom.Init(randomStateWrapper);
    }
}