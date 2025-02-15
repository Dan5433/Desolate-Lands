using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    [SerializeField] Interact interact;

    private void Update()
    {   
        if (!Input.GetKeyDown(KeyCode.Escape) || DeathMenu.IsDead || interact.IsUIActive)
            return;

        if (!GameManager.IsGamePaused)
            Pause();
        else
            Unpause();
    }

    void Pause()
    {
        GameManager.TogglePauseState();

        pauseMenu.SetActive(true);
    }

    public void Unpause()
    {
        GameManager.TogglePauseState();

        pauseMenu.SetActive(false);
    }
}
