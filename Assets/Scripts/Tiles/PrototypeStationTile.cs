using System;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
[CreateAssetMenu(menuName = "2D/Tiles/Prototype Station Tile")]
public class PrototypeStationTile : BreakableTile
{
    [SerializeField] PrototypingStationType type;

    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
    {
        if(go == null) return false;

        var script = go.GetComponent<PrototypeStation>();

        script.Type = type;
        script.CraftingUi = CraftingManager.Instance.PrototypingUI;

        return true;
    }
}
