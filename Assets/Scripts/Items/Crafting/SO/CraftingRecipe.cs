using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Crafting/Recipe")]
public class CraftingRecipe : ScriptableObject
{
    public CraftItem[] cost;
    public CraftItem reward;
}
