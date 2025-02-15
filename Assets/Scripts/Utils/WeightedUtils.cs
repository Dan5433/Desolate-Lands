using CustomClasses;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WeightedUtils
{
    public static TileBase RollTile(WeightedTileById[] tiles, TileBase[] masterList)
    {
        int totalWeight = 0;
        foreach (var tile in tiles) 
            totalWeight += tile.weight;

        int randomWeight = Random.Range(0, totalWeight);
        foreach (var tile in tiles)
        {
            randomWeight -= tile.weight;
            if (randomWeight < 0)
                return masterList[tile.tileId];
        }
        return null;
    }

    public static Tilemap RollStructure(WeightedStructure[] structures)
    {
        int totalWeight = 0;
        foreach (var structure in structures) 
            totalWeight += structure.weight;

        int randomWeight = Random.Range(0, totalWeight);
        foreach (var structure in structures)
        {
            randomWeight -= structure.weight;
            if (randomWeight < 0)
                return structure.structure;
        }
        return null;
    }
}
