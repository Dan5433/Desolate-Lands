using CustomClasses;
using CustomExtensions;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerDamage : MonoBehaviour
{
    [SerializeField][Tooltip("In Seconds")] float baseCooldown;
    [SerializeField] InventoryBase toolInventory;
    [SerializeField][Tooltip("Mutiplier")] float correctToolBonus;
    [SerializeField] AudioSource audioSource;
    float cooldown = 0;

    public void ResetCooldown()
    {
        cooldown = baseCooldown;
    }

    public void DealDamage(RaycastHit2D hit, IDamageable damageable)
    {
        if (cooldown > 0)
        {
            cooldown -= Time.deltaTime;
            return;
        }

        if (hit.transform.TryGetComponent<Tilemap>(out var tilemap))
        {
            var tile = tilemap.GetTile<BreakableTile>(
                tilemap.WorldToCell(hit.collider.transform.position));

            Break(damageable, tile);
        }
        else
        {
            Attack(damageable);
        }
    }

    void Attack(IDamageable damageable)
    {
        //TODO: add zombie hurt code here
        Debug.Log("hit living"+damageable);
    }

    void Break(IDamageable damageable, BreakableTile tile)
    {
        var equippedTool = toolInventory.Inventory[0].ItemObj as Tool;
        if (equippedTool == null /*|| 
            * tile.MinMaterial > equippedTool.Material*/)
            return;

        audioSource.PlayRandomClip(tile.BreakingAudio);

        damageable.Damage(CalculateStructureDamage(equippedTool.Material, equippedTool.Type == tile.Tool));
        UpdateDurability();

        cooldown = baseCooldown;
    }

    float CalculateStructureDamage(ItemMaterial material, bool isCorrectTool)
    {
        int baseDamage = (int)material * 2 + 1;
        return isCorrectTool ? baseDamage * correctToolBonus : baseDamage;
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
