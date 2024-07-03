using UnityEngine;
using UnityEngine.EventSystems;

public class SwapItem : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] bool detectClicks = true;
    [SerializeField] SlotType slotType;
    [SerializeField] Sprite placeholderImage;

    public Sprite Placeholder { get { return placeholderImage;  } }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!detectClicks) return;

        ItemManager.GrabItem(transform, (int)eventData.button, slotType);
    }
}
