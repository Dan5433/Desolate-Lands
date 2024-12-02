using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
[CreateAssetMenu(menuName = "Items/Consumable")]
public class ConsumableItem : Item
{
    [SerializeField] protected UnityEvent<GameObject> consume;
    public void Consume(GameObject consumer)
    {
        consume.Invoke(consumer);
    }
}
