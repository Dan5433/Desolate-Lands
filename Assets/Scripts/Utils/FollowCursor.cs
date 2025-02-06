using UnityEngine;

public class FollowCursor : MonoBehaviour
{
    void Update()
    {
        if (GameManager.IsGamePaused)
            return;

        transform.position = Input.mousePosition;
    }
}
