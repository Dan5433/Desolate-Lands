using System;
using System.Collections.Generic;
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
    static string editorWorldSuffix = " [editor]";

    public void Create()
    {
        string name = ParseName();
        string seed = ParseSeed(name);

        CreateStatsFile(name, seed);

        GameManager.PendingWorldName = name;
        GameManager.PendingSeed = "Seed: " + seed;
        SceneManager.LoadScene("Game");
    }

    void CreateStatsFile(string worldName, string seed)
    {
        var date = DateTime.Now;
        WorldStats stats = new()
        {
            creationDate = new(date.Year, date.Month, date.Day),
            seed = seed,
        };

        var worldPath = Path.Combine(MainMenuManager.SavesDirPath, worldName);

        BinaryDataHandler handler = new(worldPath, MainMenuManager.StatsFileName);
        handler.SaveData(writer => stats.Write(writer));
    }

    string ParseName()
    {
        string name = !string.IsNullOrWhiteSpace(worldNameInput.text)
            ? worldNameInput.text.Trim()
            : "New World";

        string modifiedName = name;
        if (Application.isEditor)
            modifiedName += editorWorldSuffix;

        int existingCounter = 0;

        HashSet<string> existingWorlds = new(
            Directory.GetDirectories(MainMenuManager.SavesDirPath)
                .Select(Path.GetFileName)
                .ToHashSet(),
            StringComparer.InvariantCultureIgnoreCase
        );

        while (existingWorlds.Contains(modifiedName))
        {
            existingCounter++;
            modifiedName = $"{name} ({existingCounter})";
            if (Application.isEditor)
                modifiedName += editorWorldSuffix;
        }

        Debug.Log($"Found {existingCounter} existing worlds with same name");

        return modifiedName;
    }

    string ParseSeed(string worldName)
    {
        string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()_+-=[]{};':\",.<>?/\\|`~";

        string seed;

        if (!string.IsNullOrWhiteSpace(worldSeedInput.text))
        {
            Debug.Log("Seed succesfully parsed for world: " + worldName);

            seed = worldSeedInput.text;
        }
        else
        {
            Debug.Log("Initializing random seed for world: " + worldName);

            int length = Random.Range(1, worldSeedInput.characterLimit + 1 /* account for exclusive max */ );

            StringBuilder seedBuilder = new(length);
            for (int i = 0; i < length; i++)
            {
                int charIndex = Random.Range(0, chars.Length);
                seedBuilder.Append(chars[charIndex]);
            }

            seed = seedBuilder.ToString();
        }

        Hash128 hash = Hash128.Compute(worldSeedInput.text);
        RandomStateWrapper randomStateWrapper = new(hash);

        GameRandom.Init(randomStateWrapper);

        Debug.Log("Seed: " + seed);
        return seed;
    }
}