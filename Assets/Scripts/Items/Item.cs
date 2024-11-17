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

    public int Id => id;
    public string Name => defaultName;
    public int MaxCount => maxCount;
    public Sprite Sprite => sprite;
    public SlotType Slot => slot;
}

[Serializable]
public enum SlotType
{
    Any,
    Hand,
    Body,
    Head
}
