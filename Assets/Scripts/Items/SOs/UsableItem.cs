using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
[CreateAssetMenu(menuName = "Items/Usable (Consumable)")]
public class UsableItem : Item
{
    [SerializeField] protected UsableItemEffect[] useEffects;
    public UsableItemEffect[] Effects => useEffects;
}
