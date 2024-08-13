using CustomClasses;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    public static CraftingManager Instance;

    [SerializeField] CraftingRecipe[] recipes;
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

    public static CraftingRecipe GetSingleSlotRecipe(CraftItem input, StationType type)
    {
        foreach(var recipe in Instance.recipes)
        {
            if(recipe.cost.Length == 1 && recipe.type == type 
                && recipe.cost[0].item == input.item && recipe.cost[0].count <= input.count) return recipe;
        }
        return null;
    }

    public static HashSet<CraftingRecipe> GetCraftableRecipes(InvItem[] availableItems, StationType type)
    {
        var results = new HashSet<CraftingRecipe>();
        
        //initialize dictionary for easy lookup
        var itemsCount = new Dictionary<Item, int>();
        foreach(var item in availableItems)
        {
            itemsCount[item.ItemObj] = item.Count; 
        }

        foreach(var recipe in Instance.recipes)
        {
            if (recipe.type != type) continue;

            foreach (var item in recipe.cost)
            {
                if (!itemsCount.ContainsKey(item.item) || itemsCount[item.item] < item.count) break;

                results.Add(recipe);
            }
        }

        return results;
    }
}

[Serializable]
public enum StationType
{
    Workbench = 0,
    SpinningWheel = 1,
    Loom = 2,
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

[Serializable]
public class CraftingRecipe
{
    public CraftItem[] cost;
    public CraftItem reward;
    public StationType type;
    public float craftTime;
}
