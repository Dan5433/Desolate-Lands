using UnityEngine;

public class PlayerInventory : InventoryBase
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "Item":
                var item = collision.GetComponent<GroundItem>();
                int excess = AddToInventory(item.InvItem);

                if (excess > 0)
                {
                    item.InvItem.Count = excess;
                }
                else
                {
                    Destroy(collision.gameObject);
                }

                UpdateUI();
                SaveInventory();
                break;
        }
    }
}
