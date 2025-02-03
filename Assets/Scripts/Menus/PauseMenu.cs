using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    [SerializeField] Interact interact;
    static bool isGamePaused = false;

    public static bool IsGamePaused => isGamePaused;

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape))
            return;

        if (!interact.IsUIActive && !isGamePaused)
            Pause();
        else
            Unpause();
    }

    public void ExitToMain()
    {
        SceneManager.LoadScene(0);
    }

    void Pause()
    {
        isGamePaused = true;
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Unpause()
    {
        isGamePaused = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }
}
