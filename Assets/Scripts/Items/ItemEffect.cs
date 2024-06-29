using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public abstract class ItemEffect : ScriptableObject
{
    [SerializeField] protected UnityEvent<GameObject> whenEquipped;
    [SerializeField] protected UnityEvent<GameObject> whileEquipped;
    [SerializeField] protected UnityEvent<GameObject> whenUnequipped;

    public void Equip(GameObject player)
    {
        whenEquipped.Invoke(player);
    }
    public void Equipped(GameObject player)
    {
        whileEquipped.Invoke(player);
    }
    public void Unequip(GameObject player)
    {
        whenUnequipped.Invoke(player);
    }
}
