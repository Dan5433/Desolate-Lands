using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SwapItem))]
[CanEditMultipleObjects]
public class SwapItemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SwapItem itemSlot = (SwapItem)target;
        
        itemSlot.transform.Find("Locked").gameObject.SetActive(itemSlot.Locked);
    }
}
