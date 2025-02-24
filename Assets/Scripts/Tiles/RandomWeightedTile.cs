using CustomClasses;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

[Serializable]
[CreateAssetMenu(menuName = "2D/Tiles/Random Weighted Tile")]
public class RandomWeightedTile : Tile
{
    [SerializeField] WeightedTile[] tiles;

    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
    {
        if (go == null) return false;

        //if (go.TryGetComponent<TilePlaceInit>(out var script))
        //{
        //    script.Init(GetTile(), position);
        //}

        return true;
    }

    TileBase GetTile()
    {
        int totalWeight = 0;
        foreach (var tile in tiles) totalWeight += tile.weight;

        int randomWeight = Random.Range(0, totalWeight);
        foreach (var tile in tiles)
        {
            randomWeight -= tile.weight;
            if (randomWeight < 0)
            {
                return tile.tile;
            }
        }
        return null;
    }
}
