using EditorAttributes;
using System.Collections;
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

    private void Awake()
    {
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