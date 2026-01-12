using UnityEngine;

public class DeathMenu : MonoBehaviour
{
    static bool isDead = false;

    public static bool IsDead => isDead;

    public void Death(bool animate)
    {
        isDead = true;
        GameManager.TogglePauseState(true);

        var animator = GetComponent<Animator>();
        if (animate)
            animator.enabled = true;
        else
            animator.enabled = false;

        gameObject.SetActive(true);
    }

    public void Respawn()
    {
        isDead = false;
        GameManager.TogglePauseState(false);

        gameObject.SetActive(false);
    }

    public void ExitToMain()
    {
        isDead = false;
        GameManager.ExitToMain();
    }
}
