using UnityEngine;

public class UpgradeableItem : EquippableItem
{
    [SerializeField] int tier;
    [SerializeField] Sprite[] tierSprites;

    public int Tier => tier;
    public Sprite[] Sprites => tierSprites;
}
