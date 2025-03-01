using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraConfiner : MonoBehaviour
{
    [SerializeField] PolygonCollider2D confiner;
    Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();

        if (!confiner)
        {
            Debug.LogError("Confiner is null!");
            enabled = false;
            return;
        }
    }

    void LateUpdate()
    {
        transform.position = ConfinePositionToBounds(transform.position);
    }

    Vector3 ConfinePositionToBounds(Vector3 position)
    {
        var bounds = confiner.bounds;

        float vertExtent = cam.orthographicSize;
        float horzExtent = vertExtent * cam.aspect;

        Vector2 minPos = new(bounds.min.x + horzExtent, bounds.min.y + vertExtent);
        Vector2 maxPos = new(bounds.max.x - horzExtent, bounds.max.y - vertExtent);

        if (minPos.x > maxPos.x)
            position.x = bounds.center.x;
        else
            position.x = Mathf.Clamp(position.x, minPos.x, maxPos.x);

        if (minPos.y > maxPos.y)
            position.y = bounds.center.y;
        else
            position.y = Mathf.Clamp(position.y, minPos.y, maxPos.y);

        return position;
    }
}