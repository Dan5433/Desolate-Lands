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
        if(!Application.isPlaying) 
            return false;

        if (!go || !go.TryGetComponent<PrototypeStation>(out var prototypeStation))
            return false;

        prototypeStation.StartUp(type, CraftingManager.Instance.PrototypingUI);

        return true;
    }
}
