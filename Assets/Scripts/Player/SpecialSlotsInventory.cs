public class SpecialSlotsInventory : InventoryBase
{
    protected override void InitInventory()
    {
        foreach(var item in inventory)
        {
            if (item.ItemObj is EquippableItem)
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
