using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Crafting/SlotRecipe")]
public class SlotCraftingRecipe : ScriptableObject
{
    public CraftItem cost;
    public CraftItem reward;
    public CraftingStationType type;
    public float craftTime;
    public PlayerResource[] resourceRewards;
}
