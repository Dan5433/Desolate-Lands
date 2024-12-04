using UnityEngine;

public class EquippableItem : Item
{
    [SerializeField] EquippableItemEffect[] effects;

    public EquippableItemEffect[] Effects => effects;
}
