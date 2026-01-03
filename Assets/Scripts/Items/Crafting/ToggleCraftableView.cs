using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ToggleCraftableView : MonoBehaviour
{
    [SerializeField] Sprite craftableButtonSprite;
    [SerializeField] Sprite allButtonSprite;
    [SerializeField] Image buttonImage;
    bool onlyShowCraftable = true;

    public bool OnlyShowCraftable => onlyShowCraftable;

    private void OnEnable()
    {
        buttonImage.sprite = onlyShowCraftable ? craftableButtonSprite : allButtonSprite;
    }

    public void ToggleView()
    {
        onlyShowCraftable = !onlyShowCraftable;
        buttonImage.sprite = onlyShowCraftable ? craftableButtonSprite : allButtonSprite;

        UpdateTooltip();
    }

    public void UpdateTooltip()
    {
        string message = onlyShowCraftable ? "Show All" : "Show Only Craftable";
        GameManager.Instance.Tooltip.ShowMessage(message);
    }
}
