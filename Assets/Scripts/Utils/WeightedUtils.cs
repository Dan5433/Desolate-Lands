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

        int randomWeight = GameRandom.Range(0, totalWeight);
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

        int randomWeight = GameRandom.Range(0, totalWeight);
        foreach (var structure in structures)
        {
            randomWeight -= structure.weight;
            if (randomWeight < 0)
                return structure.structure;
        }
        return null;
    }

    public static InvItem RollItem(WeightedLootPool[] lootTable, int totalPoolWeight)
    {
        WeightedLootPool chosenPool = default;

        int randomWeight = GameRandom.Range(0, totalPoolWeight);
        foreach (var pool in lootTable)
        {
            randomWeight -= pool.weight;
            if (randomWeight < 0)
            {
                chosenPool = pool;
                break;
            }
        }

        if(chosenPool.loot.Length == 0)
        {
            Debug.LogWarning("Empty Loot Pool");
            return ItemManager.Instance.InvItemAir;
        }

        int totalItemWeight = 0;
        foreach (var item in chosenPool.loot)
            totalItemWeight += item.weight;

        randomWeight = GameRandom.Range(0, totalPoolWeight);
        foreach (var weightedItem in chosenPool.loot)
        {
            randomWeight -= weightedItem.weight;
            if (randomWeight < 0)
                return weightedItem.item.Roll();
        }

        return ItemManager.Instance.InvItemAir;
    }
}
