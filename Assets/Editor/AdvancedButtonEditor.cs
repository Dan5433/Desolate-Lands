using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(AdvancedButton))]
[CanEditMultipleObjects]
public class AdvancedButtonEditor : ButtonEditor
{
    SerializedProperty onMouseDownProperty;
    SerializedProperty onMouseUpProperty;

    protected override void OnEnable()
    {
        base.OnEnable();
        onMouseDownProperty = serializedObject.FindProperty("onMouseDown");
        onMouseUpProperty = serializedObject.FindProperty("onMouseUp");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(onMouseDownProperty);

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(onMouseUpProperty);
        serializedObject.ApplyModifiedProperties();
    }
}
