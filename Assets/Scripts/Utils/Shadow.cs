using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class Shadow : MonoBehaviour
{
    [SerializeField] Vector2 offset;
    [SerializeField] Color color = new(0,0,0,0.75f);

    private void OnValidate()
    {
        var shadow = transform.Find(nameof(Shadow));
        if (!shadow)
            shadow = CreateShadow();

        shadow.GetComponent<RectTransform>().anchoredPosition = offset;

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
        target.fontSize = source.fontSize;
        target.fontStyle = source.fontStyle;
    }
}
