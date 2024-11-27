using UnityEngine;

public class DepletableItem : EquippableItem
{
    [SerializeField] int durability;
    public int Durability => durability;
}
