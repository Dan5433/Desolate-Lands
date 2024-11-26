using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class SwapItem : MonoBehaviour, IPointerDownHandler, IPointerExitHandler, IPointerEnterHandler, IPointerMoveHandler
{
    [SerializeField] bool detectClicks = true;
    [SerializeField] bool allowDeposit = true;
    [SerializeField] bool allowWithdraw = true;
    [SerializeField] SlotType slotType;
    [SerializeField] Sprite placeholderImage;
    [SerializeField] UnityEvent onClick;

    public Sprite Placeholder => placeholderImage;

    private void Awake()
    {
        onClick.AddListener(UpdateTooltip);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!detectClicks) return;

        ItemManager.GrabItem(transform, (int)eventData.button, slotType, allowDeposit, allowWithdraw);
        onClick?.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData) => UpdateTooltip();

    void UpdateTooltip()
    {
        int index = ItemManager.GetSlotIndex(transform);
        var slotItem = transform.parent.GetComponentInParent<InventoryRef>().Inventory.Inventory[index];

        var tooltip = ItemManager.Instance.ItemTooltip;

        if (slotItem.ItemObj == ItemManager.Instance.Air)
        {
            tooltip.SetActive(false);
            return;
        }

        tooltip.GetComponentInChildren<TMP_Text>().text =
            $"{slotItem.Name} (x{slotItem.Count})\n<color=#777>{slotItem.ExtraInfo()}";

        tooltip.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ItemManager.Instance.ItemTooltip.SetActive(false);
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        ItemManager.Instance.ItemTooltip.transform.position = 
            Input.mousePosition + new Vector3(5f,5f);
    }
}
