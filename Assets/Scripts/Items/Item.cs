using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Items/Item")]
public class Item : ScriptableObject
{
    [SerializeField] int id;
    [SerializeField] string defaultName;
    [SerializeField] int maxCount;
    [SerializeField] Sprite sprite;
    [SerializeField] SlotType slot;

    public int Id { get { return id; } }
    public string Name { get { return defaultName; } }
    public int MaxCount { get { return maxCount; } }
    public Sprite Sprite { get { return sprite; } }
    public SlotType Slot { get { return slot; } }
}

[Serializable]
public enum SlotType
{
    Any,
    Hand,
    Body,
    Head
}
