using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CopyToClipboard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    TMP_Text text;

    private void Awake()
    {
        text = GetComponentInChildren<TMP_Text>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        text.fontStyle |= FontStyles.Underline;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        text.fontStyle ^= FontStyles.Underline;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        GUIUtility.systemCopyBuffer = text.text;
    }

    public void ShowCopyTooltip()
    {
        ItemManager.Instance.Tooltip.ShowMessage("Click to Copy", "You can also select manually");
    }
}
