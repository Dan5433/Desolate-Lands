using CustomClasses;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCraftingUI : MonoBehaviour
{
    [SerializeField] GameObject resourcePrefab, recipePrefab;
    [SerializeField] Transform content;
    [SerializeField] PlayerInventory inventory;
    [SerializeField] PlayerCrafting crafting;
    [SerializeField] Transform selectedPrototypeUI;
    [SerializeField] Transform materialCostUI;
    [SerializeField] Image itemImage;
    [SerializeField] TMP_Text itemName;
    CraftingRecipe selectedRecipe;

    void UpdateCostUI(CraftItem[] cost)
    {
        for (int i = 0; i < cost.Length; i++)
        {
            GameObject itemUI = i < materialCostUI.childCount
                ? materialCostUI.GetChild(i).gameObject
                : Instantiate(resourcePrefab, materialCostUI);

            itemUI.GetComponentInChildren<Image>().sprite = cost[i].item.Sprite;
            itemUI.GetComponentInChildren<TMP_Text>().text = cost[i].count.ToString();
        }
        for (int i = materialCostUI.childCount - 1; i >= cost.Length; i--)
        {
            Destroy(materialCostUI.GetChild(i).gameObject);
        }
    }

    public void SelectRecipe(CraftingRecipe recipe)
    {
        if (selectedRecipe == null) 
            ToggleTooltip();

        itemImage.sprite = recipe.reward.item.Sprite;
        itemName.text = recipe.reward.item.Name;

        UpdateCostUI(recipe.cost);

        selectedRecipe = recipe;
    }

    public void Craft()
    {
        var itemsCost = new Dictionary<Item, int>();
        foreach (var item in selectedRecipe.cost)
        {
            itemsCost.Add(item.item, item.count);
        }

        int index = 0;
        foreach (var item in inventory.Inventory)
        {
            if (itemsCost.ContainsKey(item.ItemObj))
            {
                if (itemsCost[item.ItemObj] >= item.Count)
                {
                    itemsCost[item.ItemObj] -= item.Count;
                    inventory.SetSlot(index, ItemManager.Instance.InvItemAir);
                }
                else
                {
                    int remaining = item.Count - itemsCost[item.ItemObj];

                    var updatedItem = InventoryItemFactory.Create(item.ItemObj, item.Name, remaining);
                    inventory.SetSlot(index, updatedItem);

                    itemsCost.Remove(item.ItemObj);
                }
            }

            index++;
        }

        var reward = InventoryItemFactory.Create(
            selectedRecipe.reward.item,
            selectedRecipe.reward.count);

        inventory.AddToInventory(reward);

        inventory.UpdateUI();

        UpdateAvailableCraftingUI(inventory.Inventory);
        ResetUI();
    }

    public void UpdateAvailableCraftingUI(InvItem[] inventory)
    {
        var availableRecipes = CraftingManager.GetCraftableRecipes(inventory);

        var tooltip = selectedPrototypeUI.GetChild(0).GetComponent<TMP_Text>();
        if (crafting.PrototypedRecipes.Count == 0) 
            tooltip.text = "Prototype an item to craft it yourself";
        else if (availableRecipes.Count > 0) 
            tooltip.text = "Choose an item to craft";
        else 
            tooltip.text = "You don't have enough resources to craft";

        int index = 0;
        foreach (var recipe in availableRecipes)
        {
            GameObject recipeUI = index < content.childCount
                ? content.GetChild(index).gameObject
                : Instantiate(recipePrefab, content);

            var image = recipeUI.transform.Find("Image");
            image.GetComponent<Image>().sprite = recipe.reward.item.Sprite;

            int count = recipe.reward.count;
            if (count > 1) recipeUI.GetComponentInChildren<TMP_Text>().text = count.ToString();

            recipeUI.GetComponent<Button>().onClick.AddListener(
                () => SelectRecipe(recipe));

            index++;
        }

        for (int i = content.childCount - 1; i >= availableRecipes.Count; i--)
        {
            Destroy(content.GetChild(i).gameObject);
        }
    }

    public void ResetUI()
    {
        if (selectedRecipe != null) ToggleTooltip();

        selectedRecipe = null;
    }

    void ToggleTooltip()
    {
        foreach (Transform child in selectedPrototypeUI)
        {
            child.gameObject.SetActive(!child.gameObject.activeSelf);
        }
    }

    void OnDisable()
    {
        ResetUI();
    }
}
