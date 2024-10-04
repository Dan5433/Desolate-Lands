using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    void FixedUpdate()
    {
        var mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        transform.up = new(mousePos.x - transform.position.x, mousePos.y - transform.position.y);
    }
}
