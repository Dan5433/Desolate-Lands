using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotCraftStationUI : MonoBehaviour
{
    [SerializeField] PlayerCrafting playerResources;
    [SerializeField] CraftStation currentStation;
    [SerializeField] Image progressBar;
    [SerializeField] SwapItem output;
    SlotCraftingRecipe selectedRecipe = null;
    float progress;
    bool buttonHeld;

    public void SetStation(CraftStation station)
    {
        currentStation = station;

        //ResetUI();
    }

    public void LoadState(float progress)
    {
        if(!IsInputEmpty()) this.progress = progress;

        UpdateRecipe();
    }

    public void OnInputChanged()
    {
        if (selectedRecipe != null)
        {
            ResetUI(); ResetOutput();
        }

        UpdateRecipe();
    }

    void ResetOutput()
    {
        if (!IsOutputEmpty())
            return;

        output.Locked = false;
        currentStation.Inventory[1] = ItemManager.Instance.InvItemAir;
        currentStation.UpdateUI();
        currentStation.SaveInventory();
    }

    void ResetUI()
    {
        if(IsInputEmpty()) selectedRecipe = null;
        buttonHeld = false;
        progressBar.fillAmount = 0;
        progress = 0;
        currentStation.SaveProgress(progress);
    }

    public void PressButton()
    {
        if (selectedRecipe == null || !IsOutputAvailable(selectedRecipe.reward)) return;

        buttonHeld = true;
    }

    public void ReleaseButton()
    {
        buttonHeld = false;

        if(progress > 0) currentStation.SaveProgress(progress);
    }

    bool IsInputEmpty()
    {
        return currentStation.Inventory[0].ItemObj == ItemManager.Instance.Air;
    }

    bool IsOutputEmpty()
    {
        return output.Locked || currentStation.Inventory[1].ItemObj == ItemManager.Instance.Air;
    }

    bool IsOutputAvailable(CraftItem recipeOutput)
    {
        return IsOutputEmpty() || (currentStation.Inventory[1].ItemObj == recipeOutput.item &&
            currentStation.Inventory[1].Count + recipeOutput.count <= 
            currentStation.Inventory[1].ItemObj.MaxCount);
    }

    //TODO: output item into inventory when switching input but lock output before completion
    void PreviewOutput(SlotCraftingRecipe craftingRecipe)
    {
        if (!IsOutputEmpty()) return;

        progressBar.fillAmount = progress / selectedRecipe.craftTime;

        int outputCount = Mathf.Clamp(currentStation.Inventory[0].Count * craftingRecipe.cost.count,
            1, craftingRecipe.reward.item.MaxCount);

        currentStation.Inventory[1] = InventoryItemFactory.Create(
            selectedRecipe.reward.item, selectedRecipe.reward.item.Name, outputCount);

        output.Locked = true;

        currentStation.UpdateUI();
    }

    void UpdateRecipe()
    {
        CraftItem input = new(currentStation.Inventory[0].ItemObj, currentStation.Inventory[0].Count);
        var craftingRecipe = CraftingManager.GetSingleSlotRecipe(input, currentStation.Type);

        if (craftingRecipe == null || !IsOutputAvailable(craftingRecipe.reward)) return;

        selectedRecipe = craftingRecipe;

        PreviewOutput(craftingRecipe);
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
        {
            playerResources.ChangeResourceCount(reward.type, reward.count * recipesMade);
        }

        output.Locked = false;
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
