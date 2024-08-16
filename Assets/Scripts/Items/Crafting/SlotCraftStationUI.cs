using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotCraftStationUI : MonoBehaviour
{
    [SerializeField] CraftStation currentStation;
    [SerializeField] Image progressBar;
    [SerializeField] Image output;
    [SerializeField] GameObject previewPanel;
    CraftingRecipe selectedRecipe = null;
    float progress;
    bool buttonHeld;

    public void SetStation(CraftStation station)
    {
        currentStation = station;
        ResetStation();
    }

    public void LoadState(float progress)
    {
        this.progress = progress;

        UpdateOutput();
    }

    public void OnInputChanged()
    {
        if (selectedRecipe != null) ResetStation();

        UpdateOutput();
    }

    void ResetStation()
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

        currentStation.SaveProgress(progress);
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

    void PreviewOutput(CraftingRecipe craftingRecipe)  
    {
        output.sprite = craftingRecipe.reward.item.Sprite;
        previewPanel.SetActive(true);

        progressBar.fillAmount = progress / selectedRecipe.craftTime;
    }

    void UpdateOutput()
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

        ResetStation();

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
