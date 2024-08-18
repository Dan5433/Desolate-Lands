using CustomClasses;
using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Items/Tool")]
public class Tool : Item
{
    [SerializeField] ToolType type;
    [SerializeField] ItemMaterial material;

    public ToolType Type { get { return type; } }
    public ItemMaterial Material { get { return material; } }
}
