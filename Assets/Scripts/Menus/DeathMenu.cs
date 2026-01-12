using UnityEngine;

public class DeathMenu : MonoBehaviour
{
    static bool isDead = false;

    public static bool IsDead => isDead;
    

    public void Death()
    {
        isDead = true;
        GameManager.TogglePauseState();

        deathMenu.SetActive(true);

        gameObject.SetActive(true);
    }

    public void Respawn()
    {
        isDead = false;
        GameManager.TogglePauseState();

        gameObject.SetActive(false);
    }

    public void ExitToMain()
    {
        isDead = false;
        GameManager.ExitToMain();
    }
}
