using CustomClasses;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    public static CraftingManager Instance;

    [SerializeField] SlotCraftingRecipe[] slotRecipes;
    [SerializeField] CraftingRecipe[] craftingRecipes;
    [SerializeField] CraftingPrototype[] prototypes;
    [SerializeField] GameObject singleSlotUI;
    [SerializeField] GameObject chooseItemUI;

    public GameObject SingleSlotUI { get { return singleSlotUI; } }
    public GameObject ChooseItemUI { get { return chooseItemUI; } }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public static SlotCraftingRecipe GetSingleSlotRecipe(CraftItem input, CraftingStationType type)
    {
        foreach(var recipe in Instance.slotRecipes)
        {
            if(recipe.type == type && recipe.cost.item == input.item 
                && recipe.cost.count <= input.count) return recipe;
        }
        return null;
    }

    public static HashSet<CraftingPrototype> GetCraftablePrototypes(InvItem[] availableItems, PrototypingStationType type)
    {
        var results = new HashSet<CraftingPrototype>();
        
        //initialize dictionary for easy lookup
        var itemsCount = new Dictionary<Item, int>();
        foreach(var item in availableItems)
        {
            itemsCount[item.ItemObj] = item.Count; 
        }

        foreach(var recipe in Instance.prototypes)
        {
            if (recipe.type != type) continue;

            foreach (var item in recipe.recipe.cost)
            {
                if (!itemsCount.ContainsKey(item.item) || itemsCount[item.item] < item.count) break;

                results.Add(recipe);
            }
        }

        return results;
    }
}

[Serializable]
public enum CraftingStationType
{
    Smelter = 0,
    SpinningWheel = 1,
    Loom = 2,
}

[Serializable]
public enum PrototypingStationType
{
    Workbench = 0,
}

[Serializable]
public struct CraftItem
{
    public Item item;
    public int count;

    public CraftItem(Item item, int count)
    {
        this.item = item;
        this.count = count;
    }
}