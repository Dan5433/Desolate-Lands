using System;
using System.IO;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager Instance { get; private set; }

    [SerializeField] TMP_Text versionText;
    [SerializeField] GameObject titleScreen;
    static string statsFileName = "stats.bin";
    static string savesDirPath;

    public static string StatsFileName => statsFileName;
    public static string SavesDirPath => savesDirPath;

    private void Awake()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

        savesDirPath = Path.Combine(Application.persistentDataPath, "saves");
        Directory.CreateDirectory(savesDirPath);
        versionText.text = "v" + Application.version;

        Random.InitState((int)DateTime.Now.Ticks);

        titleScreen.SetActive(false);
        titleScreen.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }
}