using System.IO;
using TMPro;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager Instance { get; private set; }

    [SerializeField] TMP_Text versionText;
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
    }
    public void Quit()
    {
        Application.Quit();
    }
}