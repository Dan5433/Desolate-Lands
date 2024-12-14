using CustomClasses;
using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Items/Tool")]
public class Tool : DepletableItem
{
    [SerializeField] ToolType type;
    [SerializeField] ItemMaterial material;
    [SerializeField] float structureDamage;
    [SerializeField] float damageSpeed;

    public ToolType Type => type;
    public ItemMaterial Material => material;
    public float StructureDamage => structureDamage;
    public float DamageSpeed => damageSpeed;
}
