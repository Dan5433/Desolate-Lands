using UnityEngine;

public class EquippableItem : Item
{
    [SerializeField] ItemEffect[] effects;

    public ItemEffect[] Effects { get { return effects; } }
}
