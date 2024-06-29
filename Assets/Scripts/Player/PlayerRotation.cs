using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    void FixedUpdate()
    {
        Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;

        transform.up = direction;
    }
}
