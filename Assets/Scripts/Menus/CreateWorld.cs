using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreateWorld : MonoBehaviour
{
    [SerializeField] TMP_InputField worldName;
    [SerializeField] TMP_InputField worldSeed;

    public void Create()
    {
        string saveDirectory = Path.Combine(Application.persistentDataPath, "saves");

        string name = worldName.text != string.Empty ? worldName.text : "New World";

        int existingCounter = 0;
        foreach (var dir in Directory.GetDirectories(saveDirectory, $"*{name}*"))
            existingCounter++;

        Debug.Log($"Found {existingCounter} existing worlds with same name");

        if (existingCounter > 0)
            name += $" ({existingCounter})";

        if (int.TryParse(worldSeed.text,out int seed))
        {
            Debug.Log("Seed succesfully parsed: "+seed);
            PlayerPrefs.SetInt(name, seed);
        }

        GameManager.LoadWorldName = name;
        SceneManager.LoadScene("Game");
    }
}