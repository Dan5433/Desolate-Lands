using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class TooltipInteract : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
{
    [SerializeField] UnityEvent pointerEnter;
    public void OnPointerEnter(PointerEventData eventData)
    {
        pointerEnter?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ItemManager.Instance.Tooltip.Hide();
    }

    private void OnDisable()
    {
        ItemManager.Instance.Tooltip.Hide();
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        ItemManager.Instance.Tooltip.UpdatePosition();
    }
}
