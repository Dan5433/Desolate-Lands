using UnityEngine;

public class InventoryRef : MonoBehaviour
{
    [SerializeField] InventoryBase inventory;

    public InventoryBase Inventory { get { return inventory; } set { inventory = value; } }
}
