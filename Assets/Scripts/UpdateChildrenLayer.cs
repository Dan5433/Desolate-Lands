using System.Collections;
using UnityEngine;

public class UpdateChildrenLayer : MonoBehaviour
{
    IEnumerator Start()
    {
        while (true)
        {
            var children = GetComponentsInChildren<Transform>(includeInactive: true);
            foreach (var child in children)
            {
                child.gameObject.layer = gameObject.layer;
            }
            yield return new WaitForSeconds(1f);
        }
    }
}
