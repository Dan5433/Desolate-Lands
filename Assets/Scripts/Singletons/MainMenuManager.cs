using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] GameObject[] panels;
    [SerializeField] GameObject activePanel;

    public void OpenWorld()
    {
        SceneManager.LoadScene(1);
    }
    public void OpenSettings()
    {
        Debug.Log("settings");
    }
    public void Quit()
    {
        Application.Quit();
    }

    public void ChangePanel(int panelIndex)
    {
        activePanel.SetActive(false);
        activePanel = panels[panelIndex];
        activePanel.SetActive(true);
    }
}