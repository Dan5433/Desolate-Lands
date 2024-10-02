using CustomClasses;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance { get; private set; }

    [SerializeField] GameObject groundItemPrefab;
    [SerializeField] GameObject invSlotPrefab;
    [SerializeField] GameObject containerUI;
    [SerializeField] ItemRef heldItem;
    [SerializeField] Item air;
    InvItem invItemAir;
    [SerializeField] Item[] items;
    public Item[] Items { get { return items; } }
    public Item Air { get { return air; } }
    public InvItem InvItemAir { get { return invItemAir; } }
    public GameObject ContainerUI { get { return containerUI; } }
    public GameObject InvSlot { get { return invSlotPrefab; } }
    public bool IsHoldingItem { get { return heldItem.Item.ItemObj != Air; } }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            invItemAir = new(air, air.Name, 0);
            heldItem.Item = invItemAir;
            Instance = this;
        }
    }

    public static void DropHeldItem(Vector3 position, Vector3 direction, float distance)
    {
        if (Instance.heldItem.Item.ItemObj == Instance.Air) return;

        Vector3 spawnPos = position + direction * distance;

        LayerMask mask = LayerMask.GetMask("Interact");
        RaycastHit2D hit = Physics2D.Raycast(position, direction, distance, mask);

        if (hit) spawnPos = hit.point;

        SpawnGroundItem(Instance.heldItem.Item, spawnPos, false);
        Instance.heldItem.Item = Instance.InvItemAir;
        UpdateItemUI(Instance.heldItem.transform, Instance.heldItem.Item);
    }

    public static bool IsInvSlot(GameObject gameObject)
    {
        if (gameObject.GetComponent<SwapItem>() != null) return true;
        return false;
    }

    public static void UpdateItemUI(Transform slot, InvItem item)
    {
        var image = slot.Find("Image").GetComponent<Image>();
        var countTxt = slot.GetComponentInChildren<TMP_Text>();

        if (item.ItemObj == Instance.air)
        {
            if (slot.TryGetComponent<SwapItem>(out var script))
            {
                image.sprite = script.Placeholder;
            }
            else
            {
                image.sprite = Instance.air.Sprite;
            }
        }
        else
        {
            image.sprite = item.ItemObj.Sprite;
        }

        if (item.Count > 1)
        {
            countTxt.text = item.Count.ToString();
        }
        else
        {
            countTxt.text = string.Empty;
        }
    }

    public static void SpawnGroundItem(InvItem item, Vector3 middlePos, bool randomizePos)
    {
        if (item.ItemObj == Instance.Air) return;

        var groundItem = Instantiate(Instance.groundItemPrefab, GameManager.Instance.WorldCanvas.transform);
        groundItem.GetComponent<GroundItem>().InvItem = item;
        Vector3 pos = middlePos;

        if (randomizePos)
        {
            RectTransform rect = groundItem.GetComponent<RectTransform>();
            pos = middlePos + new Vector3(
            Random.Range(rect.sizeDelta.x / 2, 1 - rect.sizeDelta.x / 2),
            Random.Range(rect.sizeDelta.y / 2, 1 - rect.sizeDelta.y / 2));
        }

        groundItem.transform.position = pos;
    }

    static void DepositItem(ref InvItem slotItem, InvItem selectedItem)
    {
        //add if sum is less or equal to max count
        if (slotItem.Count + selectedItem.Count <= slotItem.ItemObj.MaxCount)
        {
            slotItem.Count += selectedItem.Count;
            Instance.heldItem.Item = Instance.InvItemAir;
        }
        //deposit the maximum amount into the slot if greater than max count
        else
        {
            Instance.heldItem.Item.Count -= slotItem.ItemObj.MaxCount - slotItem.Count;
            slotItem.Count = slotItem.ItemObj.MaxCount;
        }
    }

    static void SplitSlot(ref InvItem slotItem)
    {
        int half = slotItem.Count / 2;
        Instance.heldItem.Item = new(slotItem.ItemObj, slotItem.Name, half);

        if (slotItem.Count >= 2)
        {
            slotItem.Count -= half;
        }
        else
        {
            slotItem = Instance.InvItemAir;
        }
    }

    static void IncrementSlot(ref InvItem slotItem, InvItem selectedItem)
    {
        //add item data if slot is empty
        if (slotItem.ItemObj == Instance.Air)
        {
            slotItem = new(selectedItem.ItemObj, selectedItem.Name, 0);
        }

        slotItem.Count++;
        if (Instance.heldItem.Item.Count <= 1)
        {
            Instance.heldItem.Item = Instance.InvItemAir;
        }
        else
        {
            Instance.heldItem.Item.Count--;
        }
    }

    static bool SwapSlots(ref InvItem slotItem, InvItem selectedItem, bool allowDeposit, bool allowWithdraw)
    {
        if (slotItem.ItemObj == Instance.Air && selectedItem.ItemObj == Instance.Air) return false;
        if (!allowDeposit && slotItem.ItemObj == Instance.Air) return false;
        if (!allowWithdraw && selectedItem.ItemObj == Instance.Air) return false;

        Instance.heldItem.Item = new(slotItem);
        slotItem = new(selectedItem);
        return true;
    }

    static int GetSlotIndex(Transform slot)
    {
        int index = 0;
        var parent = slot.parent;
        for (int i = 0; i < parent.childCount; i++)
        {
            var child = parent.GetChild(i);

            if (child == slot) break;
            if (IsInvSlot(child.gameObject)) index++;
        }
        return index;
    }

    public static void GrabItem(Transform slot, int mouseButton, SlotType slotType, bool allowDeposit, bool allowWithdraw)
    {
        var slotInventory = slot.parent.GetComponentInParent<InventoryRef>().Inventory;

        int index = GetSlotIndex(slot);
        InvItem slotItem = slotInventory.Inventory[index];
        InvItem selectedItem = new(Instance.heldItem.Item);

        if (selectedItem.ItemObj != Instance.Air && slotType != SlotType.Any && selectedItem.ItemObj.Slot != slotType) return;

        if (slotType != SlotType.Any) SpecialItemActions(slotItem.ItemObj, selectedItem.ItemObj, GameManager.Instance.Player);

        if (mouseButton == 0)
        {
            if (slotItem.ItemObj == selectedItem.ItemObj && slotItem.ItemObj != Instance.Air)
            {
                if (allowDeposit) DepositItem(ref slotItem, selectedItem);
                else return;
            }

            else
            {
                if (!SwapSlots(ref slotItem, selectedItem, allowDeposit, allowWithdraw)) return;
            }
        }

        else if (mouseButton == 1)
        {
            if (selectedItem.ItemObj == Instance.Air && slotItem.ItemObj != Instance.Air)
            {
                if (allowWithdraw) SplitSlot(ref slotItem);
                else return;
            }
            else if (slotItem.ItemObj == Instance.Air || slotItem.ItemObj == selectedItem.ItemObj)
            {
                if (allowDeposit) IncrementSlot(ref slotItem, selectedItem);
                else return;
            }

            else
            {
                if(!SwapSlots(ref slotItem, selectedItem, allowDeposit, allowWithdraw)) return;
            }
        }

        slotInventory.SetSlot(index, slotItem);

        UpdateItemUI(slot, slotItem);
        UpdateItemUI(Instance.heldItem.transform, Instance.heldItem.Item);

        slotInventory.SaveInventory();
    }

    static void SpecialItemActions(Item slot, Item selected, GameObject invoker)
    {
        if (slot != null && slot.GetType() == typeof(EquippableItem))
        {
            var item = slot as EquippableItem;
            foreach (var effect in item.Effects)
            {
                effect.Unequip(invoker);
            }
        }
        if (selected != null && selected.GetType() == typeof(EquippableItem))
        {
            var item = selected as EquippableItem;
            foreach (var effect in item.Effects)
            {
                effect.Equip(invoker);
            }
        }
    }
}
