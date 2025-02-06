using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathMenu : MonoBehaviour
{
    [SerializeField] GameObject deathMenu;
    static bool isDead = false;

    public static bool IsDead => isDead;
    

    public void Death()
    {
        isDead = true;
        GameManager.TogglePauseState();

        deathMenu.SetActive(true);
    }

    public void Respawn()
    {
        isDead = false;
        GameManager.TogglePauseState();

        deathMenu.SetActive(false);
    }
}
