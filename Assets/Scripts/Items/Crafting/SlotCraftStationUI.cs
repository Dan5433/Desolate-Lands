using UnityEngine;
using UnityEngine.UI;

public class SlotCraftStationUI : MonoBehaviour
{
    [SerializeField] CraftStation currentStation;
    [SerializeField] Image progressBar;
    CraftingRecipe selectedRecipe;
    float progress;
    bool buttonHeld;

    public void SetStation(CraftStation station)
    {
        currentStation = station;
    }

    public void SetProgress(float progress)
    {
        this.progress = progress;
    }

    public void ResetStation()
    {
        selectedRecipe = null;
        buttonHeld = false;
        progressBar.fillAmount = 0;
        progress = 0;
    }

    public void ReleaseButton()
    {
        buttonHeld = false;

        currentStation.SaveProgress(progress);
    }

    bool IsOutputAvailable(CraftItem recipeOutput)
    {
        return currentStation.Inventory[1].ItemObj == ItemManager.Instance.Air ||
            (currentStation.Inventory[1].ItemObj == recipeOutput.item &&
            currentStation.Inventory[1].Count + recipeOutput.count <= currentStation.Inventory[1].ItemObj.MaxCount);
    }

    public void UpdateOutput()
    {
        if (buttonHeld) return;

        CraftItem input = new(currentStation.Inventory[0].ItemObj, currentStation.Inventory[0].Count);
        var craftingRecipe = CraftingManager.GetSingleSlotRecipe(input, currentStation.Type);

        if (craftingRecipe == null || !IsOutputAvailable(craftingRecipe.reward)) return;

        selectedRecipe = craftingRecipe;
        buttonHeld = true;
    }

    void OutputReward()
    {
        int maxOutput = (selectedRecipe.reward.item.MaxCount - currentStation.Inventory[1].Count) / 
            selectedRecipe.reward.count;

        currentStation.Inventory[1] = new(selectedRecipe.reward.item, selectedRecipe.reward.item.Name, 0);

        if (maxOutput < currentStation.Inventory[0].Count / selectedRecipe.cost[0].count)
        {
            currentStation.Inventory[1].Count += maxOutput * selectedRecipe.reward.count;
            currentStation.Inventory[0].Count -= maxOutput * selectedRecipe.cost[0].count;
        }
        else
        {
            currentStation.Inventory[1].Count += currentStation.Inventory[0].Count * selectedRecipe.reward.count;
            currentStation.Inventory[0] = ItemManager.Instance.InvItemAir;
        }

        currentStation.UpdateUI();

        ResetStation();
    }

    void Update()
    {
        if (!buttonHeld) return;

        progress += Time.deltaTime;
        progressBar.fillAmount = progress / selectedRecipe.craftTime;

        if (progress >= selectedRecipe.craftTime) OutputReward();
    }
}
