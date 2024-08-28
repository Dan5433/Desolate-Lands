using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Crafting/Prototype")]
public class CraftingPrototype : ScriptableObject
{
    public CraftingRecipe recipe;
    public ResourceCost[] resourceCost;
}

[Serializable]
public struct ResourceCost
{
    public CraftingResource resource;
    public int count;
}
