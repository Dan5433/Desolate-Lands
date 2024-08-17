using System.Collections;
using UnityEngine;

public class UpdateChildrenLayer : MonoBehaviour
{
    void OnTransformChildrenChanged()
    {
        var children = GetComponentsInChildren<Transform>(includeInactive: true);
        foreach (var child in children)
        {
            child.gameObject.layer = gameObject.layer;
        }
    }
}
