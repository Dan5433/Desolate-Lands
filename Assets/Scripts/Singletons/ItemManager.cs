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
    [SerializeField] GameObject itemTooltip;
    [SerializeField] ItemRef heldItem;
    [SerializeField] Item air;
    InvItem invItemAir;
    [SerializeField] Item[] items;
    public Item[] Items => items;
    public Item Air => air;
    public InvItem InvItemAir => invItemAir;
    public GameObject ContainerUI => containerUI;
    public GameObject ItemTooltip => itemTooltip;
    public GameObject InvSlot => invSlotPrefab;
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
        GameManager.CursorState = CursorState.Default;

        Vector3 spawnPos = position + direction * distance;

        LayerMask mask = LayerMask.GetMask("Interact");
        RaycastHit2D hit = Physics2D.Raycast(position, direction, distance, mask);

        if (hit) spawnPos = hit.point;


        SpawnGroundItem(Instance.heldItem.Item.Clone(), spawnPos, false);

        Instance.heldItem.Item = Instance.InvItemAir;
        UpdateItemUI(Instance.heldItem.transform, Instance.heldItem.Item);
    }

    public static bool IsInvSlot(GameObject gameObject)
    {
        if (gameObject.GetComponent<SwapItem>() != null) return true;
        return false;
    }

    public static bool IsHeldItemUsable()
    {
        if (Instance.heldItem.Item.ItemObj is UsableItem) return true;
        return false;
    }

    public static void UseHeldItem(GameObject user)
    {
        if (!IsHeldItemUsable()) return;

        var item = Instance.heldItem.Item.ItemObj as UsableItem;

        foreach(var effect in item.Effects)
        {
            effect.Use(user);
        }

        if(Instance.heldItem.Item.Count > 1)
        {
            Instance.heldItem.Item.Count--;
        }
        else
        {
            Instance.heldItem.Item = Instance.InvItemAir;
        }

        UpdateItemUI(Instance.heldItem.gameObject.transform, Instance.heldItem.Item);
        if(!Instance.IsHoldingItem) GameManager.CursorState = CursorState.Default;
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

        if (item.CountTxt > 1)
        {
            countTxt.text = item.CountTxt.ToString();
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

    static void DepositItem(InvItem slotItem, InvItem selectedItem)
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
        Instance.heldItem.Item = InventoryItemFactory.Create(
            slotItem.ItemObj, slotItem.Name, half);

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
            slotItem = selectedItem.Clone();
            slotItem.Count = 0;
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

        Instance.heldItem.Item = slotItem.Clone();
        slotItem = selectedItem.Clone();
        return true;
    }

    public static int GetSlotIndex(Transform slot)
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
        var slotItem = slotInventory.Inventory[index];
        var selectedItem = Instance.heldItem.Item.Clone();

        if (selectedItem.ItemObj != Instance.Air && slotType != SlotType.Any && selectedItem.ItemObj.Slot != slotType) return;

        if (slotType != SlotType.Any) SpecialItemActions(slotItem.ItemObj, selectedItem.ItemObj, GameManager.Instance.Player);

        if (mouseButton == 0)
        {
            if (slotItem.ItemObj == selectedItem.ItemObj && slotItem.ItemObj != Instance.Air)
            {
                if (allowDeposit) DepositItem(slotItem, selectedItem);
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
            else if (slotItem.ItemObj == Instance.Air || 
                (slotItem.ItemObj == selectedItem.ItemObj && slotItem.Count < slotItem.ItemObj.MaxCount))
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
