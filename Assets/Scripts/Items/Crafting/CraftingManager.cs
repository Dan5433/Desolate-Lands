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
        var station = Array.Find(Instance.slotRecipes, r => r.type == type);

        foreach (var recipe in station.recipes)
        {
            if (recipe.cost.item == input.item
                && recipe.cost.count <= input.count) return recipe;
        }
        return null;
    }

    public static Dictionary<CraftingPrototype, MissingRequirements> GetPrototypesAndMissing(InvItem[] availableItems, PlayerResource[] availableResources, PrototypeStationType type)
    {
        Dictionary<CraftingPrototype, MissingRequirements> results = new();

        var itemsCount = GetAvailableItemsDict(availableItems);

        var resourcesCount = new Dictionary<Resource, int>();
        foreach (var resource in availableResources)
        {
            resourcesCount[resource.type] = resource.count;
        }

        var station = Array.Find(Instance.prototypes, p => p.type == type);

        foreach (var prototype in station.prototypes)
        {
            if (HasPrototypedBefore(prototype.recipe))
                continue;

            results.Add(prototype, new() { missingItems = new(), missingResources = new()});

            foreach (var item in prototype.recipe.cost)
            {
                if (itemsCount.ContainsKey(item.item) &&
                    itemsCount[item.item] >= item.count)
                    continue;

                results[prototype].missingItems.Add(item.item);
            }

            foreach (var resource in prototype.resourceCost)
            {
                if (resourcesCount[resource.resource.type] >= resource.count)
                    continue;

                results[prototype].missingResources.Add(resource.resource.type);
            }
        }

        return results;
    }

    public static HashSet<CraftingRecipe> GetCraftableRecipes(InvItem[] availableItems)
    {
        var results = new HashSet<CraftingRecipe>();

        var itemsCount = GetAvailableItemsDict(availableItems);

        foreach (var recipe in Instance.crafting.PrototypedRecipes)
        {
            bool canCraft = true;
            foreach (var item in recipe.cost)
            {
                if (itemsCount.ContainsKey(item.item) &&
                    itemsCount[item.item] >= item.count)
                    continue;

                canCraft = false; break;
            }

            if (canCraft) results.Add(recipe);
        }
        return results;
    }

    static Dictionary<Item, int> GetAvailableItemsDict(InvItem[] availableItems)
    {
        //initialize dictionary for easy lookup
        var itemsCount = new Dictionary<Item, int>();
        foreach (var item in availableItems)
        {
            if (!itemsCount.ContainsKey(item.ItemObj)) itemsCount[item.ItemObj] = 0;

            itemsCount[item.ItemObj] += item.Count;
        }
        return itemsCount;
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
        public PrototypeStationType type;
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
public enum PrototypeStationType
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

[Serializable]
public struct MissingRequirements
{
    public HashSet<Item> missingItems;
    public HashSet<Resource> missingResources;
}