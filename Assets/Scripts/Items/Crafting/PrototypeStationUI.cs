using CustomClasses;
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
    CraftingPrototype selectedPrototype;
    PrototypeStation currentStation;

    public void SetStation(PrototypeStation station)
    {
        currentStation = station;

        DefaultState();
    }

    void UpdateMaterialsUI(CraftItem[] cost)
    {
        var materialsUI = costUI.Find("Materials");

        for (int i = 0; i < cost.Length; i++)
        {
            GameObject itemUI;

            if (i < materialsUI.childCount) itemUI = materialsUI.GetChild(i).gameObject;
            else itemUI = Instantiate(resourcePrefab, materialsUI);

            itemUI.GetComponentInChildren<Image>().sprite = cost[i].item.Sprite;
            itemUI.GetComponentInChildren<TMP_Text>().text = cost[i].count.ToString();
        }
        for (int i = materialsUI.childCount - 1; i >= cost.Length; i--)
        {
            Destroy(materialsUI.GetChild(i).gameObject);
        }
    }

    void UpdateResourcesUI(ResourceCost[] cost)
    {
        var resourcesUI = costUI.Find("Resources");

        for (int i = 0; i < cost.Length; i++)
        {
            GameObject resourceCost;

            if (i < resourcesUI.childCount) resourceCost = resourcesUI.GetChild(i).gameObject;
            else resourceCost = Instantiate(resourcePrefab, resourcesUI);

            resourceCost.GetComponentInChildren<Image>().sprite = cost[i].resource.sprite;
            resourceCost.GetComponentInChildren<TMP_Text>().text = cost[i].count.ToString();
        }

        for (int i = resourcesUI.childCount - 1; i >= cost.Length; i--)
        {
            Destroy(resourcesUI.GetChild(i).gameObject);
        }
    }

    public void SelectPrototype(CraftingPrototype prototype)
    {
        if (selectedPrototype == null) ToggleTooltip();

        itemImage.sprite = prototype.recipe.reward.item.Sprite;

        UpdateMaterialsUI(prototype.recipe.cost);
        UpdateResourcesUI(prototype.resourceCost);

        selectedPrototype = prototype;
    }

    public void Prototype()
    {
        foreach (var resource in selectedPrototype.resourceCost)
        {
            crafting.AddResource(resource.resource.type, -resource.count);
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

                    InvItem updatedItem = new(item.ItemObj, item.Name, remaining);
                    inventory.SetSlot(index, updatedItem);

                    itemsCost.Remove(item.ItemObj);
                }
            }

            index++;
        }

        InvItem reward = new(selectedPrototype.recipe.reward.item, 
            selectedPrototype.recipe.reward.item.Name, 
            selectedPrototype.recipe.reward.count);

        int excess = inventory.AddToInventory(reward);

        if(excess > 0)
        {
            reward.Count = excess;
            ItemManager.SpawnGroundItem(reward, GameManager.Instance.Player.transform.position, false);
        }

        inventory.UpdateUI();

        crafting.UnlockRecipe(selectedPrototype.recipe);

        currentStation.UpdateAvailablePrototypesUI(inventory.Inventory, crafting.Resources);
        DefaultState();
    }

    public void DefaultState()
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
