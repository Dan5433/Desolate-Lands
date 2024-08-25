using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Crafting/Prototype")]
public class CraftingPrototype : ScriptableObject
{
    public CraftingRecipe recipe;
    public PrototypingStationType type;
    public PlayerResource[] resourceCost;
}
