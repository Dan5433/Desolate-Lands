using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class SwapItem : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] bool detectClicks = true;
    [SerializeField] bool allowDeposit = true;
    [SerializeField] bool allowWithdraw = true;
    [SerializeField] SlotType slotType;
    [SerializeField] Sprite placeholderImage;
    [SerializeField] UnityEvent onClick;

    public Sprite Placeholder => placeholderImage;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!detectClicks) return;

        ItemManager.GrabItem(transform, (int)eventData.button, slotType, allowDeposit, allowWithdraw);
        onClick?.Invoke();
    }
}
