using CustomClasses;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WeightedUtils
{
    public static Tile RollTile(WeightedTileById[] tiles, Tile[] masterList)
    {
        int totalWeight = 0;
        foreach (var tile in tiles) totalWeight += tile.Weight;

        int randomWeight = Random.Range(0, totalWeight);
        foreach (var tile in tiles)
        {
            randomWeight -= tile.Weight;
            if (randomWeight < 0)
            {
                return masterList[tile.TileId];
            }
        }
        return null;
    }

    public static Tilemap RollStructure(WeightedStructure[] structures)
    {
        int totalWeight = 0;
        foreach (var structure in structures) totalWeight += structure.Weight;

        int randomWeight = Random.Range(0, totalWeight);
        foreach (var structure in structures)
        {
            randomWeight -= structure.Weight;
            if (randomWeight < 0)
            {
                return structure.Structure;
            }
        }
        return null;
    }
}
