using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
[ExecuteInEditMode]
[DisallowMultipleComponent]
public class Shadow : MonoBehaviour
{
    [SerializeField] Vector2 offset;
    [SerializeField] Color color = new(0,0,0,0.75f);

    private void OnValidate()
    {
        var shadow = transform.Find(nameof(Shadow));
        if (!shadow)
            shadow = CreateShadow();

        var rectTransform = shadow.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.anchoredPosition = offset;

        var text = shadow.GetComponent<TextMeshProUGUI>();
        text.color = color;

        CopyTextProperties(GetComponent<TextMeshProUGUI>(), text);
    }

    Transform CreateShadow()
    {
        GameObject shadow = new(nameof(Shadow), typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        shadow.transform.SetParent(transform, false);
        shadow.layer = LayerMask.NameToLayer("UI");

        //Add canvas component with sorting override to draw shadow behind text
        var newCanvas = shadow.AddComponent<Canvas>();
        newCanvas.vertexColorAlwaysGammaSpace = true;
        newCanvas.overrideSorting = true;
        newCanvas.sortingOrder = -1;

        return shadow.transform;
    }

    void CopyTextProperties(TextMeshProUGUI source, TextMeshProUGUI target)
    {
        target.text = source.text;
        target.margin = source.margin;
        target.alignment = source.alignment;
        target.enableWordWrapping = source.enableWordWrapping;
        target.overflowMode = source.overflowMode;
        target.font = source.font;
        target.enableAutoSizing = source.enableAutoSizing;
        target.fontSizeMin = source.fontSizeMin;
        target.fontSizeMax = source.fontSizeMax;
        target.fontSize = source.fontSize;
        target.fontStyle = source.fontStyle;
    }

    private void OnDestroy()
    {
        if (Application.isPlaying)
            return;

        DestroyImmediate(transform.Find(nameof(Shadow)).gameObject);
    }
}
