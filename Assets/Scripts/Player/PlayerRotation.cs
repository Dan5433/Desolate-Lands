using CustomExtensions;
using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    [SerializeField] PlayerMovement movement;
    [SerializeField][Tooltip("In Degrees")] int fieldOfReach;
    void FixedUpdate()
    {
        var mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        Vector2 direction = new(mousePos.x - transform.position.x, mousePos.y - transform.position.y);

        Quaternion mouseRotation = Quaternion.LookRotation(Vector3.forward, direction);

        int facingDirection = (int)movement.Facing;

        Quaternion baseRotation = Quaternion.Euler(new(0, 0, facingDirection * 90f));

        float angleDifference = Quaternion.Angle(baseRotation, mouseRotation);
        if(angleDifference > fieldOfReach / 2)
        {
            float sign = Mathf.Sign(Vector2.SignedAngle(baseRotation * Vector2.up, direction));
            mouseRotation = baseRotation * Quaternion.Euler(0,0, sign * fieldOfReach / 2);
        }
            
        transform.rotation = mouseRotation;
    }
}
