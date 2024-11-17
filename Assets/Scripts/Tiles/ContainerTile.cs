using CustomClasses;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
[CreateAssetMenu(menuName = "2D/Tiles/Container Tile")]
public class ContainerTile : BreakableTile
{
    [SerializeField] WeightedItem[] lootPool;
    [SerializeField] int invSize;

    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
    {
        if (go == null) return false;

        var script = go.GetComponent<Container>();
        script.enabled = true;

        script.LootPool = lootPool;

        return true;
    }
}
