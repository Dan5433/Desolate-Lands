using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotCraftStationUI : MonoBehaviour
{
    [SerializeField] PlayerResources playerResources;
    [SerializeField] CraftStation currentStation;
    [SerializeField] Image progressBar;
    [SerializeField] Image output;
    [SerializeField] GameObject previewPanel;
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
        if(!IsInputEmpty()) this.progress = progress;

        UpdateRecipe();
    }

    public void OnInputChanged()
    {
        if (selectedRecipe != null) ResetUI();

        UpdateRecipe();
    }

    void ResetUI()
    {
        if(IsInputEmpty()) selectedRecipe = null;
        buttonHeld = false;
        progressBar.fillAmount = 0;
        progress = 0;

        output.sprite = ItemManager.Instance.Air.Sprite;
        previewPanel.SetActive(false);
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

    bool IsOutputAvailable(CraftItem recipeOutput)
    {
        return currentStation.Inventory[1].ItemObj == ItemManager.Instance.Air ||
            (currentStation.Inventory[1].ItemObj == recipeOutput.item &&
            currentStation.Inventory[1].Count + recipeOutput.count <= currentStation.Inventory[1].ItemObj.MaxCount);
    }

    void PreviewOutput(SlotCraftingRecipe craftingRecipe)  
    {
        output.sprite = craftingRecipe.reward.item.Sprite;
        previewPanel.SetActive(true);

        progressBar.fillAmount = progress / selectedRecipe.craftTime;
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
        int maxOutput = (selectedRecipe.reward.item.MaxCount - currentStation.Inventory[1].Count) / 
            selectedRecipe.reward.count;

        currentStation.Inventory[1] = new(selectedRecipe.reward.item, selectedRecipe.reward.item.Name, 0);

        if (maxOutput < currentStation.Inventory[0].Count / selectedRecipe.cost.count)
        {
            foreach (var reward in selectedRecipe.resourceRewards)
            {
                playerResources.AddResource(reward.resource, reward.count * maxOutput);
            }

            currentStation.Inventory[1].Count += maxOutput * selectedRecipe.reward.count;
            currentStation.Inventory[0].Count -= maxOutput * selectedRecipe.cost.count;
        }
        else
        {
            foreach (var reward in selectedRecipe.resourceRewards)
            {
                playerResources.AddResource(reward.resource, reward.count * currentStation.Inventory[0].Count);
            }

            currentStation.Inventory[1].Count += currentStation.Inventory[0].Count * selectedRecipe.reward.count;
            currentStation.Inventory[0] = ItemManager.Instance.InvItemAir;
        }

        ResetUI();

        currentStation.UpdateUI();
    }

    void Update()
    {
        if (!buttonHeld) return;

        progress += Time.deltaTime;
        progressBar.fillAmount = progress / selectedRecipe.craftTime;

        if (progress >= selectedRecipe.craftTime) OutputReward();
    }
}
