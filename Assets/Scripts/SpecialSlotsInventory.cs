using UnityEngine;

public class SpecialSlotsInventory : InventoryBase
{
    protected override void LoadInventory()
    {
        base.LoadInventory();
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
