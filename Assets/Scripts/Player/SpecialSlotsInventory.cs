using System.Threading.Tasks;
using UnityEngine;

public class SpecialSlotsInventory : InventoryBase
{
    protected override void InitInventory(InventorySaveData data)
    {
        base.InitInventory(data);
        foreach (var item in inventory) 
        {
            if(item.ItemObj.GetType() == typeof(EquippableItem))
            {
                var equippable = item.ItemObj as EquippableItem;
                foreach (var effect in equippable.Effects)
                {
                    effect.Equip(GameManager.Instance.Player);
                }
            }
        }
    }
}
