using CustomClasses;
using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Items/Tool")]
public class Tool : DepletableItem
{
    [SerializeField] ToolType type;
    [SerializeField] ItemMaterial material;
    [SerializeField] float attackDamage;
    [SerializeField] float attackSpeed;

    public ToolType Type => type;
    public ItemMaterial Material => material;
    public float AttackDamage => attackDamage;
    public float AttackSpeed => attackSpeed;

}