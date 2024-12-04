using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInventory : InventoryBase
{
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
}