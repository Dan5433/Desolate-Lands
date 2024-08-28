using System;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
[CreateAssetMenu(menuName = "2D/Tiles/Craft Station Tile")]
public class PrototypingStationTile : BreakableTile
{
    [SerializeField] CraftingStationType type;

    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
    {
        if(go == null) return false;

        var script = go.GetComponent<CraftStation>();
        script.CraftingUi = CraftingManager.Instance.CraftingUI;

        script.Type = type;

        script.enabled = true;

        return true;
    }
}
