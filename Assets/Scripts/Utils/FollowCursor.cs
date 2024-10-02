using UnityEngine;

public class FollowCursor : MonoBehaviour
{
    void Update()
    {
        transform.position = Input.mousePosition;
    }
}
