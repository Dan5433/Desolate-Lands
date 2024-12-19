using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class SwapItem : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] bool detectClicks = true;
    [SerializeField] bool allowDeposit = true;
    [SerializeField] bool allowWithdraw = true;
    [SerializeField] bool locked = false;
    [SerializeField] SlotType slotType;
    [SerializeField] Sprite placeholderImage;
    [SerializeField] UnityEvent onClick;

    public Sprite Placeholder => placeholderImage;
    public bool Locked {  get { return locked; } 
        set 
        {  
            locked = value; 
            transform.Find("Locked").gameObject.SetActive(value);
        } 
    }

    private void Awake()
    {
        onClick.AddListener(UpdateTooltip);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!detectClicks || locked) return;

        ItemManager.GrabItem(transform, (int)eventData.button, slotType, allowDeposit, allowWithdraw);
        onClick?.Invoke();
    }

    public void UpdateTooltip()
    {
        int index = ItemManager.GetSlotIndex(transform);
        var slotItem = transform.parent.GetComponentInParent<InventoryRef>().Inventory.Inventory[index];

        if (slotItem.ItemObj == ItemManager.Instance.Air)
        {
            ItemManager.Instance.Tooltip.Hide();
            return;
        }

        ItemManager.Instance.Tooltip.ShowMessage(
            $"{slotItem.Name} (x{slotItem.Count})", 
            slotItem.ExtraInfo());
    }
}
