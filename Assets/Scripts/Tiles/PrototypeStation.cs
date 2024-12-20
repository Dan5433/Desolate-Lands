using CustomClasses;
using CustomExtensions;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PrototypeStation : MonoBehaviour
{
    [SerializeField] GameObject prototypePrefab;
    PrototypeStationType type;
    GameObject craftingUi;

    public GameObject CraftingUi => craftingUi;

    public void StartUp(PrototypeStationType type, GameObject ui)
    {
        this.type = type;
        craftingUi = ui;
    }

    public void UpdateAvailablePrototypesUI(InvItem[] inventory, PlayerResource[] resources)
    {
        var uiScript = CraftingUi.GetComponent<PrototypeStationUI>();
        uiScript.SetStation(this);

        var scrollView = CraftingUi.GetComponentInChildren<ScrollRect>().transform;
        var content = scrollView.GetComponentInChildren<GridLayoutGroup>().transform;

        var prototypesAndMissing = CraftingManager.GetPrototypesAndMissing(inventory, resources, type);

        GameObject prototypeUI;
        int index = 0;
        foreach (var prototype in prototypesAndMissing)
        {
            if (uiScript.OnlyShowCraftable &&
                (prototype.Value.missingItems.Count > 0 ||
                prototype.Value.missingResources.Count > 0))
                continue;

            if (index < content.childCount) prototypeUI = content.GetChild(index).gameObject;
            else prototypeUI = Instantiate(prototypePrefab, content);

            var image = prototypeUI.transform.Find("Image");
            image.GetComponent<Image>().sprite = prototype.Key.recipe.reward.item.Sprite;

            int count = prototype.Key.recipe.reward.count;
            var countText = prototypeUI.GetComponentInChildren<TMP_Text>();
            if (count > 1) countText.text = count.ToString();
            else countText.text = string.Empty;

            var chooseEvent = prototypeUI.GetComponent<Button>().onClick;
            chooseEvent.RemoveAllListeners();
            chooseEvent.AddListener(
                    () => uiScript.SelectPrototype(prototype.Key, prototype.Value));

            var newPulse = prototypeUI.transform.Find("NewPulse").gameObject;
            if(newPulse.activeInHierarchy) 
                newPulse.GetComponent<ImageColorPulse>().ResetPulse();

            newPulse.SetActive(
                prototype.Value.missingItems.Count == 0 && 
                prototype.Value.missingResources.Count == 0);

            index++;
        }

        if (index > 0) uiScript.Tooltip.text = "Choose an item to prototype";
        else uiScript.Tooltip.text = "You don't have enough resources to prototype something new";

        for (int i = content.childCount - 1; i >= index; i--)
        {
            Destroy(content.GetChild(i).gameObject);
        }
    }
}