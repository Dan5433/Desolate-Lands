using CustomClasses;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInventory : InventoryBase
{
    [SerializeField] UnityEvent onInventoryItemAdd;
    void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "Item":
                var item = collision.GetComponent<GroundItem>();

                if(AddToInventory(item.InvItem.Clone()) <= 0)
                {
                    Destroy(collision.gameObject);
                }

                UpdateUI();
                SaveInventory();
                break;
        }
    }

    public override void ClearInventory()
    {
        base.ClearInventory();
        DeleteInventory();
    }

    public override int AddToInventory(InvItem item)
    {
        int excess = base.AddToInventory(item);
        onInventoryItemAdd?.Invoke();
        return excess;
    }
}