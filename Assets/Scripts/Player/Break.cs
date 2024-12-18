using CustomClasses;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Break : MonoBehaviour
{
    float cooldown = 0;

    [SerializeField][Tooltip("In Seconds")] float breakCooldown;
    [SerializeField] InventoryBase toolInventory;
    [SerializeField][Tooltip("Mutiplier")] float correctToolBonus;

    public void ResetBreaking()
    {
        cooldown = 0;
    }

    public void Breaking(IDamageable damageable, BreakableTile tile)
    {
        if (cooldown > 0)
        {
            cooldown -= Time.deltaTime;
            return;
        }

        var equippedTool = toolInventory.Inventory[0].ItemObj as Tool;
        if (equippedTool == null /*|| 
            * tile.MinMaterial > equippedTool.Material*/ ||
            ItemManager.Instance.IsHoldingItem)
            return;

        damageable.Damage(CalculateDamage((int)equippedTool.Material, equippedTool.Type == tile.Tool));
        cooldown = breakCooldown;
    }

    float CalculateDamage(int damage, bool isCorrectTool)
    {
        return isCorrectTool ? damage * correctToolBonus : damage;
    }

    void UpdateDurability()
    {
        var equippedTool = toolInventory.Inventory[0] as InvTool;
        equippedTool.Durability -= 1;

        if (equippedTool.Durability <= 0) toolInventory.Inventory[0] = ItemManager.Instance.InvItemAir;
        toolInventory.SaveInventory();
        toolInventory.UpdateUI();
    }
}
