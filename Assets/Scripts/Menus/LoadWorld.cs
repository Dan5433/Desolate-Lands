using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadWorld : MonoBehaviour
{
    [SerializeField] Transform scrollContent;
    [SerializeField] Transform worldOptions;
    [SerializeField] Transform worldSelectPrefab;
    GameObject selectedWorld;

    private void Awake()
    {
        foreach (var dir in Directory.GetDirectories(MainMenuManager.SavesDirPath))
        {
            WorldStats stats = new();
            BinaryDataHandler handler = new(dir, MainMenuManager.StatsFileName);
            handler.LoadData(reader => stats = new(reader));

            //seed update for versions 1.1.0-alpha+
            if (stats.seed == null)
            {
                stats.seed = string.Empty;
                handler.SaveData(writer => stats.Write(writer));
            }

            Transform worldSelect = Instantiate(worldSelectPrefab, scrollContent);
            worldSelect.GetComponent<Button>().onClick.AddListener(EnableWorldOptions);
            worldSelect.GetComponentInChildren<TMP_Text>().text = Path.GetFileName(dir);

            var statsContainer = worldSelect.Find("Stats");
            statsContainer.Find("Playtime").GetComponent<TMP_Text>().text =
                "Playtime: " + stats.playtime;

            statsContainer.Find("Creation Date").GetComponent<TMP_Text>().text =
                "Creation Date: " + stats.creationDate.ToShortDateString();

            statsContainer.Find("Deaths").GetComponent<TMP_Text>().text =
                "Deaths: " + stats.deaths;

            statsContainer.Find("Seed").GetComponent<TMP_Text>().text =
                !string.IsNullOrEmpty(stats.seed)
                ? "Seed: " + stats.seed
                : string.Empty;
        }
    }

    private void Update()
    {
        if (!selectedWorld)
            DisableWorldOptions();

        if (!EventSystem.current.currentSelectedGameObject)
        {
            selectedWorld = null;
            return;
        }

        if (EventSystem.current.currentSelectedGameObject.transform.parent == scrollContent)
            selectedWorld = EventSystem.current.currentSelectedGameObject;
    }

    public void EnableWorldOptions()
    {
        foreach (var button in worldOptions.GetComponentsInChildren<Button>())
        {
            button.interactable = true;
            button.GetComponentInChildren<TMP_Text>().color = Color.white;
        }
    }
    public void DisableWorldOptions()
    {
        foreach (var button in worldOptions.GetComponentsInChildren<Button>())
        {
            button.interactable = false;
            button.GetComponentInChildren<TMP_Text>().color = button.colors.disabledColor;
        }
    }

    public void LoadSelectedWorld()
    {
        string worldName = selectedWorld.GetComponentInChildren<TMP_Text>().text;

        var statsContainer = selectedWorld.transform.Find("Stats");
        string seed = statsContainer.Find("Seed").GetComponent<TMP_Text>().text;
        if (seed != string.Empty)
            seed = seed["Seed: ".Length..];

        GameManager.InitializeWorld(worldName, seed);
        SceneManager.LoadScene("Game");
    }

    public void DeleteSelectedWorld()
    {
        string worldName = selectedWorld.GetComponentInChildren<TMP_Text>().text;

        string worldDirPath = Path.Combine(MainMenuManager.SavesDirPath, worldName);
        PlayerPrefs.DeleteKey(worldName);
        Directory.Delete(worldDirPath, true);

        Destroy(selectedWorld);
        selectedWorld = null;
    }
}

[Serializable]
public struct WorldStats
{
    public Playtime playtime;
    public DateTime creationDate;
    public int deaths;
    public string seed;

    public readonly void Write(BinaryWriter writer)
    {
        writer.Write(playtime.hours);
        writer.Write(playtime.minutes);
        writer.Write(playtime.seconds);
        writer.Write(creationDate.Year);
        writer.Write(creationDate.Month);
        writer.Write(creationDate.Day);
        writer.Write(deaths);
        writer.Write(seed);
    }

    public WorldStats(BinaryReader reader)
    {
        playtime = new(reader);
        creationDate = new(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
        deaths = reader.ReadInt32();
        seed = reader.ReadString();
    }
}

[Serializable]
public struct Playtime
{
    public int hours;
    public int minutes;
    public int seconds;

    public Playtime(BinaryReader reader)
    {
        hours = reader.ReadInt32();
        minutes = reader.ReadInt32();
        seconds = reader.ReadInt32();
    }
    public override readonly string ToString()
    {
        return $"{hours:D2}h {minutes:D2}m {seconds:D2}s";
    }
}