using CustomClasses;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    public static CraftingManager Instance;

    [SerializeField] Recipes[] slotRecipes;
    [SerializeField] Prototypes[] prototypes;
    [SerializeField] CraftingRecipe[] recipes;
    [SerializeField] PlayerCrafting crafting;
    [SerializeField] GameObject craftingUI;
    [SerializeField] GameObject prototypingUI;

    public GameObject CraftingUI => craftingUI;
    public GameObject PrototypingUI => prototypingUI;

    public CraftingRecipe[] CraftingRecipes => recipes;

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

    public static bool HasPrototypedBefore(CraftingRecipe recipe)
    {
        return Instance.crafting.PrototypedRecipes.Contains(recipe);
    }

    public static SlotCraftingRecipe GetSingleSlotRecipe(CraftItem input, CraftingStationType type)
    {
        var matching = Array.Find(Instance.slotRecipes, r => r.type == type);

        foreach(var recipe in matching.recipes)
        {
            if(recipe.cost.item == input.item 
                && recipe.cost.count <= input.count) return recipe;
        }
        return null;
    }

    public static HashSet<CraftingPrototype> GetCraftablePrototypes(InvItem[] availableItems, PlayerResource[] availableResources, PrototypingStationType type)
    {
        var results = new HashSet<CraftingPrototype>();
        
        //initialize dictionary for easy lookup
        var itemsCount = new Dictionary<Item, int>();
        foreach(var item in availableItems)
        {
            if(!itemsCount.ContainsKey(item.ItemObj)) itemsCount[item.ItemObj] = 0;

            itemsCount[item.ItemObj] += item.Count; 
        }

        var resourcesCount = new Dictionary<Resource, int>();
        foreach (var resource in availableResources)
        {
            resourcesCount[resource.type] = resource.count;
        }

        var matching = Array.Find(Instance.prototypes, p => p.type == type);

        foreach(var recipe in matching.prototypes)
        {
            bool available = true;
            foreach (var item in recipe.recipe.cost)
            {
                if (!itemsCount.ContainsKey(item.item) || itemsCount[item.item] < item.count)
                {
                    available = false;
                    break;
                }
            }

            foreach (var resource in recipe.resourceCost)
            {
                if (!available) break;

                if (resourcesCount[resource.resource.type] < resource.count)
                {
                    available = false;
                    break;
                }
            }

            if (available) results.Add(recipe);
        }
        return results;
    }

    public static HashSet<CraftingRecipe> GetCraftableRecipes(InvItem[] availableItems)
    {
        var results = new HashSet<CraftingRecipe>();

        //initialize dictionary for easy lookup
        var itemsCount = new Dictionary<Item, int>();
        foreach (var item in availableItems)
        {
            if(!itemsCount.TryAdd(item.ItemObj, item.Count)) 
                itemsCount[item.ItemObj]+= item.Count;
        }

        foreach (var recipe in Instance.crafting.PrototypedRecipes)
        {
            bool canCraft = true;
            foreach (var item in recipe.cost)
            {
                if (!itemsCount.ContainsKey(item.item) || itemsCount[item.item] < item.count)
                {
                    canCraft = false; break;
                }
                    
            }

            if(canCraft) results.Add(recipe);
        }
        return results;
    }

    [Serializable]
    struct Recipes
    {
        public CraftingStationType type;
        public SlotCraftingRecipe[] recipes;
    }

    [Serializable]
    struct Prototypes
    {
        public PrototypingStationType type;
        public CraftingPrototype[] prototypes;
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