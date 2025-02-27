using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PrototypeStationUI : MonoBehaviour
{
    [SerializeField] GameObject resourcePrefab;
    [SerializeField] PlayerInventory inventory;
    [SerializeField] PlayerCrafting crafting;
    [SerializeField] Transform selectedPrototypeUI;
    [SerializeField] Transform costUI;
    [SerializeField] Image itemImage;
    [SerializeField] TMP_Text itemName;
    [SerializeField] ToggleCraftableView craftableView;
    [SerializeField] Button prototypeButton;
    [SerializeField] Color missingColor;
    CraftingPrototype selectedPrototype;
    PrototypeStation currentStation;

    public TMP_Text Tooltip => selectedPrototypeUI.GetChild(0).GetComponent<TMP_Text>();
    public bool OnlyShowCraftable => craftableView.OnlyShowCraftable;

    public void SetStation(PrototypeStation station)
    {
        currentStation = station;

        ResetUI();
    }

    public void UpdateUI()
    {
        if (!currentStation)
            return;

        currentStation.UpdateAvailablePrototypesUI(inventory.Inventory, crafting.Resources);
    }

    void UpdateMaterialsUI(CraftItem[] cost, HashSet<Item> missing)
    {
        var materialsUI = costUI.Find("Materials");

        for (int i = 0; i < cost.Length; i++)
        {
            GameObject resourceUI = i < materialsUI.childCount
                ? materialsUI.GetChild(i).gameObject
                : Instantiate(resourcePrefab, materialsUI);

            resourceUI.GetComponentInChildren<Image>().sprite = cost[i].item.Sprite;

            var text = resourceUI.GetComponentInChildren<TMP_Text>();
            if (missing.Contains(cost[i].item))
                text.text = 
                    $"<color=#{ColorUtility.ToHtmlStringRGB(missingColor)}>" + cost[i].count;
            else
                text.text = cost[i].count.ToString();
        }
        for (int i = materialsUI.childCount - 1; i >= cost.Length; i--)
        {
            Destroy(materialsUI.GetChild(i).gameObject);
        }
    }
    void UpdateResourcesUI(PlayerResource[] cost, HashSet<Resource> missing)
    {
        var resourcesUI = costUI.Find("Resources");

        for (int i = 0; i < cost.Length; i++)
        {
            GameObject resourceCost = i < resourcesUI.childCount
                ? resourcesUI.GetChild(i).gameObject
                : Instantiate(resourcePrefab, resourcesUI);

            resourceCost.GetComponentInChildren<Image>().sprite = cost[i].resource.sprite;

            var text = resourceCost.GetComponentInChildren<TMP_Text>();
            if (missing.Contains(cost[i].resource.type))
                text.text =
                    $"<color=#{ColorUtility.ToHtmlStringRGB(missingColor)}>{cost[i].count}";
            else
                text.text = cost[i].count.ToString();
        }

        for (int i = resourcesUI.childCount - 1; i >= cost.Length; i--)
        {
            Destroy(resourcesUI.GetChild(i).gameObject);
        }
    }

    public void SelectPrototype(CraftingPrototype prototype, MissingRequirements missing)
    {
        if(selectedPrototype == prototype) 
            return;

        if (selectedPrototype == null)
            ToggleTooltip();

        selectedPrototype = prototype;

        itemImage.sprite = prototype.recipe.reward.item.Sprite;
        itemName.text = prototype.recipe.reward.item.Name;

        UpdateMaterialsUI(prototype.recipe.cost, missing.missingItems);
        UpdateResourcesUI(prototype.resourceCost, missing.missingResources);

        if(missing.missingResources.Count > 0 || missing.missingItems.Count > 0)
            prototypeButton.interactable = false;
        else
            prototypeButton.interactable = true;

    }

    public void Prototype()
    {
        foreach (var resource in selectedPrototype.resourceCost)
        {
            crafting.ChangeResourceCount(resource.resource.type, -resource.count);
        }

        var itemsCost = new Dictionary<Item, int>();
        foreach (var item in selectedPrototype.recipe.cost)
        {
            itemsCost[item.item] = item.count;
        }

        int index = 0;
        foreach(var item in inventory.Inventory)
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
            selectedPrototype.recipe.reward.item, 
            selectedPrototype.recipe.reward.count);

        inventory.AddToInventory(reward);

        inventory.UpdateUI();

        crafting.UnlockRecipe(selectedPrototype.recipe);

        currentStation.UpdateAvailablePrototypesUI(inventory.Inventory, crafting.Resources);
        ResetUI();
    }

    public void ResetUI()
    {
        if (selectedPrototype != null) ToggleTooltip();

        selectedPrototype = null;
    }

    void ToggleTooltip()
    {
        foreach (Transform child in selectedPrototypeUI)
        {
            child.gameObject.SetActive(!child.gameObject.activeSelf);
        }
    }
    
}
