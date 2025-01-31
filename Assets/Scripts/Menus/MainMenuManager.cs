using EditorAttributes;
using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager Instance { get; private set; }

    [SerializeField] TMP_Text versionText;
    [SerializeField] GameObject[] panels;
    [SerializeField] GameObject activePanel;
    static string statsFileName = "stats.bin";
    static string savesDirPath;

    public static string StatsFileName => statsFileName;
    public static string SavesDirPath => savesDirPath;

    private void Awake()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        ChangePanel(0);

        savesDirPath = Path.Combine(Application.persistentDataPath, "saves");
        Directory.CreateDirectory(savesDirPath);
        versionText.text = "v" + Application.version;
    }

    public void OpenSettings()
    {
        Debug.Log("settings");
    }
    public void Quit()
    {
        Application.Quit();
    }

    [Button("Change Panel",42)]
    public void ChangePanel(int panelIndex)
    {
        activePanel.SetActive(false);
        activePanel = panels[panelIndex];
        activePanel.SetActive(true);
    }
}