using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Crafting/Resource")]
public class CraftingResource : ScriptableObject
{
    public Resource type;
    public Sprite sprite;
}
