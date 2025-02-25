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
        if(!Application.isPlaying) 
            return false;

        if (!go || !go.TryGetComponent<CraftStation>(out var craftStation))
            return false;

        craftStation.StartUp(type, CraftingManager.Instance.CraftingUI);
        craftStation.enabled = true;

        return true;
    }
}
