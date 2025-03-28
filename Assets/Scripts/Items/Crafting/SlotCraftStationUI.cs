using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotCraftStationUI : MonoBehaviour
{
    [SerializeField] PlayerCrafting playerResources;
    [SerializeField] CraftStation currentStation;
    [SerializeField] Image progressBar;
    [SerializeField] SwapItem outputSlot;
    [SerializeField] Transform resourceRewards;
    [SerializeField] GameObject resourcePrefab;
    SlotCraftingRecipe selectedRecipe = null;
    float progress;
    bool buttonHeld;

    public void SetStation(CraftStation station)
    {
        currentStation = station;

        ResetUI();
    }

    public void LoadState(float progress)
    {
        if(!IsInputEmpty()) 
            this.progress = progress;

        UpdateRecipe();
    }

    public void OnInputChanged()
    {
        if (selectedRecipe != null)
        {
            ResetUI(); 
            ResetOutput();
            currentStation.SaveProgress(progress);
        }

        UpdateRecipe();
    }

    void ResetOutput()
    {
        if (!IsOutputEmpty())
            return;

        outputSlot.Locked = false;
        currentStation.Inventory[1] = ItemManager.Instance.InvItemAir;
        currentStation.UpdateUI();
        currentStation.SaveInventory();
    }

    void ResetUI()
    {
        selectedRecipe = null;
        buttonHeld = false;
        progressBar.fillAmount = 0;
        progress = 0;
        resourceRewards.gameObject.SetActive(false);
    }

    public void PressButton()
    {
        if (selectedRecipe == null || !IsOutputAvailable(selectedRecipe.reward.item)) 
            return;

        buttonHeld = true;
    }

    public void ReleaseButton()
    {
        buttonHeld = false;

        if(progress > 0) 
            currentStation.SaveProgress(progress);
    }

    bool IsInputEmpty()
    {
        return currentStation.Inventory[0].ItemObj == ItemManager.Instance.Air;
    }

    bool IsOutputEmpty()
    {
        return outputSlot.Locked || currentStation.Inventory[1].ItemObj == ItemManager.Instance.Air;
    }

    bool IsOutputAvailable(Item reward)
    {
        return currentStation.Inventory[1].ItemObj == reward && outputSlot.Locked 
            || currentStation.Inventory[1].ItemObj == ItemManager.Instance.Air;
    }

    void PreviewItemOutput(SlotCraftingRecipe craftingRecipe)
    {
        progressBar.fillAmount = progress / selectedRecipe.craftTime;

        int outputCount = Mathf.Clamp(currentStation.Inventory[0].Count * craftingRecipe.cost.count,
            1, craftingRecipe.reward.item.MaxCount);

        currentStation.Inventory[1] = InventoryItemFactory.Create(
            selectedRecipe.reward.item, selectedRecipe.reward.item.Name, outputCount);

        currentStation.UpdateUI();
    }

    void PreviewResourcesOutput(SlotCraftingRecipe craftingRecipe)
    {
        int amount = craftingRecipe.resourceRewards.Length;

        for (int i = 0; i < amount; i++)
        {
            var reward = craftingRecipe.resourceRewards[i];

            GameObject resourceUI = i < resourceRewards.childCount
            ? resourceRewards.GetChild(i).gameObject
            : Instantiate(resourcePrefab, resourceRewards);

            int total = reward.count * currentStation.Inventory[1].Count;
            resourceUI.GetComponentInChildren<Image>().sprite = reward.resource.sprite;
            resourceUI.GetComponentInChildren<TMP_Text>().text = total.ToString();
        }
        for (int i = resourceRewards.childCount - 1; i >= amount; i--)
        {
            Destroy(resourceRewards.GetChild(i).gameObject);
        }

        resourceRewards.gameObject.SetActive(true);
    }

    void UpdateRecipe()
    {
        CraftItem input = new(currentStation.Inventory[0].ItemObj, currentStation.Inventory[0].Count);
        var craftingRecipe = CraftingManager.GetSingleSlotRecipe(input, currentStation.Type);

        if (craftingRecipe == null)
        {
            outputSlot.Locked = false;
            return;
        }
        else
            outputSlot.Locked = true;

        if (!IsOutputEmpty() || !IsOutputAvailable(craftingRecipe.reward.item))
            return;

        selectedRecipe = craftingRecipe;

        PreviewItemOutput(craftingRecipe);
        PreviewResourcesOutput(craftingRecipe);
    }

    void OutputReward()
    {
        int recipesMade = currentStation.Inventory[1].Count / selectedRecipe.reward.count;

        currentStation.Inventory[0].Count -= recipesMade * selectedRecipe.cost.count;

        if(currentStation.Inventory[0].Count <= 0)
        {
            currentStation.Inventory[0] = ItemManager.Instance.InvItemAir;
        }

        foreach (var reward in selectedRecipe.resourceRewards)
            playerResources.ChangeResourceCount(reward.resource.type, reward.count * recipesMade);

        outputSlot.Locked = false;
        ResetUI();

        currentStation.SaveProgress(progress);
        currentStation.UpdateUI();
        currentStation.SaveInventory();
    }

    void Update()
    {
        if (!buttonHeld) return;

        progress += Time.deltaTime;
        progressBar.fillAmount = progress / selectedRecipe.craftTime;

        if (progress >= selectedRecipe.craftTime) OutputReward();
    }
}
