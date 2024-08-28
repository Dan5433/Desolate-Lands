using CustomClasses;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PrototypeStation : MonoBehaviour
{
    [SerializeField] GameObject prototypePrefab;
    public PrototypingStationType Type;
    public GameObject CraftingUi;

    public void UpdateAvailablePrototypesUI(InvItem[] inventory, PlayerResource[] resources)
    {
        var uiScript = CraftingUi.GetComponent<PrototypeStationUI>();
        uiScript.SetStation(this);

        var scrollView = CraftingUi.GetComponentInChildren<ScrollRect>().transform;
        var content = scrollView.GetComponentInChildren<GridLayoutGroup>().transform;

        var availablePrototypes = CraftingManager.GetCraftablePrototypes(inventory, resources, Type);

        int index = 0;
        foreach(var prototype in availablePrototypes)
        {
            GameObject prototypeUI;

            if (index < content.childCount) prototypeUI = content.GetChild(index).gameObject;
            else prototypeUI = Instantiate(prototypePrefab, content);

            var image = prototypeUI.transform.Find("Image");
            image.GetComponent<Image>().sprite = prototype.recipe.reward.item.Sprite;

            int count = prototype.recipe.reward.count;
            if(count > 1) prototypeUI.GetComponentInChildren<TMP_Text>().text = count.ToString();

            prototypeUI.GetComponent<Button>().onClick.AddListener(
                () => uiScript.SelectPrototype(prototype));

            index++;
        }
        for (int i = content.childCount - 1; i >= availablePrototypes.Count; i--)
        {
            Destroy(content.GetChild(i).gameObject);
        }
    }
}
