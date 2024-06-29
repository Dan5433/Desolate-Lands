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
    [SerializeField] ItemRef grabbedItem;
    [SerializeField] Item air;
    [SerializeField] Item[] items;
    public Item[] Items { get { return items; } }
    public Item Air { get { return air; } }
    public GameObject ContainerUI { get { return containerUI; } }
    public GameObject InvSlot { get { return invSlotPrefab; } }
    public bool ItemGrabbed { get {  return grabbedItem.Item.ItemObj != Air; } }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            grabbedItem.Item = new(air, air.Name, air.MaxCount);
            Instance = this;
        }
    }

    public static void DropHeldItem(Vector3 position, Vector3 direction, float distance)
    {
        if (Instance.grabbedItem.Item.ItemObj == Instance.Air) return;

        Vector3 spawnPos = position + direction*distance;

        LayerMask mask = LayerMask.GetMask("Solid");
        RaycastHit2D hit = Physics2D.Raycast(position, direction, distance, mask);

        if(hit) spawnPos = hit.point;

        SpawnGroundItem(Instance.grabbedItem.Item, spawnPos, false);
        Instance.grabbedItem.Item = new(Instance.Air, Instance.Air.Name, Instance.Air.MaxCount);
        UpdateItemUI(Instance.grabbedItem.transform, Instance.grabbedItem.Item);
    }

    public static void UpdateItemUI(Transform slot, InvItem item)
    {
        var image = slot.Find("Image").GetComponent<Image>();
        var countTxt = slot.GetComponentInChildren<TMP_Text>();

        if(item.ItemObj == Instance.air )
        {
            if(slot.TryGetComponent<SwapItem>(out var script))
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

    public static void GrabItem(Transform slot, int mouseButton, SlotType slotType)
    {
        var slotInventory = slot.parent.GetComponent<InventoryRef>().Inventory;

        int index = slot.GetSiblingIndex();
        InvItem slotItem = slotInventory.Inventory[index];
        InvItem selectedItem = new(Instance.grabbedItem.Item);

        if (selectedItem.ItemObj != Instance.Air && slotType != SlotType.Any && selectedItem.ItemObj.Slot != slotType) return;

        if(slotType != SlotType.Any) SpecialItemActions(slotItem.ItemObj, selectedItem.ItemObj, GameManager.Instance.Player);

        if (mouseButton == 0)
        {
            if(slotItem.ItemObj == selectedItem.ItemObj && slotItem.ItemObj != Instance.Air) 
            {
                if(slotItem.Count + selectedItem.Count <= slotItem.ItemObj.MaxCount) 
                {
                    slotItem.Count += selectedItem.Count;
                    Instance.grabbedItem.Item = new(Instance.Air, Instance.Air.Name, Instance.Air.MaxCount);
                }
                else
                {
                    Instance.grabbedItem.Item.Count -= slotItem.ItemObj.MaxCount - slotItem.Count;
                    slotItem.Count = slotItem.ItemObj.MaxCount;
                }
            }
            else
            {
                Instance.grabbedItem.Item = new(slotItem);
                slotItem = new(selectedItem);
            }
        }
        else if (mouseButton == 1) 
        {
            if(selectedItem.ItemObj == Instance.Air && slotItem.ItemObj != Instance.Air) 
            {
                int half = slotItem.Count / 2;
                Instance.grabbedItem.Item = new(slotItem.ItemObj,slotItem.Name,half);

                if(slotItem.Count >= 2)
                {
                    slotItem.Count -= half;
                }
                else
                {
                    slotItem = new(Instance.Air,Instance.Air.Name,Instance.Air.MaxCount);
                }
            }
            else if (slotItem.ItemObj == Instance.Air || slotItem.ItemObj == selectedItem.ItemObj)
            {
                if (slotItem.ItemObj == Instance.Air) 
                {
                    slotItem = new(selectedItem.ItemObj, selectedItem.Name, 0);
                }

                slotItem.Count++;
                if (Instance.grabbedItem.Item.Count <= 1)
                {
                    Instance.grabbedItem.Item = new(Instance.Air, Instance.Air.Name, Instance.Air.MaxCount);
                }
                else
                {
                    Instance.grabbedItem.Item.Count--;
                }
            }
            else
            {
                Instance.grabbedItem.Item = new(slotItem);
                slotItem = new(selectedItem);
            }
        }

        slotInventory.SetSlot(index, slotItem);

        UpdateItemUI(slot, slotItem);
        UpdateItemUI(Instance.grabbedItem.transform, Instance.grabbedItem.Item);

        slotInventory.SaveInventory();
    }

    static void SpecialItemActions(Item slot, Item selected, GameObject invoker)
    {
        if (slot != null && slot.GetType() == typeof(EquippableItem))
        {
            var item = slot as EquippableItem;
            foreach(var effect in item.Effects)
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
