using System;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
[CreateAssetMenu(menuName = "2D/Tiles/Prototype Station Tile")]
public class PrototypeStationTile : BreakableTile
{
    [SerializeField] PrototypeStationType type;

    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
    {
        if(go == null) return false;

        go.GetComponent<PrototypeStation>().StartUp(type, CraftingManager.Instance.PrototypingUI);

        return true;
    }
}
