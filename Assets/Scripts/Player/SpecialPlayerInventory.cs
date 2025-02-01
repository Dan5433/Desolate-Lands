public class SpecialPlayerInventory : InventoryBase
{
    protected override void InitInventory()
    {
        foreach(var item in inventory)
        {
            if (item.ItemObj is EquippableItem equippable)
            {
                foreach (var effect in equippable.Effects)
                    effect.Equip(GameManager.Instance.Player);
            }
        }
    }

    public override void ClearInventory()
    {
        base.ClearInventory();
        DeleteInventory();
    }
    protected override string GetSaveKey()
    {
        return base.GetSaveKey() + " special";
    }
}
