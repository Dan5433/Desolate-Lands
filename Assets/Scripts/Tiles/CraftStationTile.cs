using System;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
[CreateAssetMenu(menuName = "2D/Tiles/Craft Station Tile")]
public class CraftStationTile : BreakableTile
{
    [SerializeField] CraftingStationType type;
    [SerializeField] UIType uiType;

    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
    {
        if(go == null) return false;

        var script = go.GetComponent<CraftStation>();
        script.CraftingUi = uiType == UIType.Slot ? CraftingManager.Instance.SingleSlotUI :
            CraftingManager.Instance.ChooseItemUI;

        script.Type = type;

        script.enabled = true;

        return true;
    }

    enum UIType
    {
        Slot = 0,
        Choose = 1,
    }
}
