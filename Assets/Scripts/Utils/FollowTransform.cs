using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] bool copyRotation = false;

    private void Update()
    {
        if(copyRotation) transform.SetPositionAndRotation(
            new(target.position.x, target.position.y, transform.position.z), 
            target.rotation);
        else
        {
            transform.position = new(target.position.x, target.position.y, transform.position.z);
        }
    }
}
