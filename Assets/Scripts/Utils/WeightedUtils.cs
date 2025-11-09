using CustomClasses;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WeightedUtils
{
    public static TileBase RollTile(WeightedTileById[] tiles, TileBase[] masterList)
    {
        SeededRandom random = RNGManager.Instance.Generators.worldgen;

        int totalWeight = 0;
        foreach (var tile in tiles)
            totalWeight += tile.weight;

        int randomWeight = random.Range(0, totalWeight);
        foreach (var tile in tiles)
        {
            randomWeight -= tile.weight;
            if (randomWeight < 0)
                return masterList[tile.tileId];
        }
        return null;
    }

    public static TileBase RollPerlinNoiseTile(PerlinNoiseTerrainTiles tiles, TileBase[] masterList, Vector2 tilePosition)
    {
        int totalWeight = 0;
        foreach (var tile in tiles.tiles)
            totalWeight += tile.weight;

        float xCoord = tilePosition.x / TerrainManager.ChunkSize.x * tiles.scale;
        float yCoord = tilePosition.y / TerrainManager.ChunkSize.y * tiles.scale;
        float sample = Mathf.PerlinNoise(xCoord, yCoord);

        float randomWeight = Mathf.Clamp01(sample) * totalWeight;
        foreach (var tile in tiles.tiles)
        {
            randomWeight -= tile.weight;
            if (randomWeight < 0)
                return masterList[tile.tileId];
        }
        return null;
    }

    public static Tilemap RollStructure(WeightedStructure[] structures)
    {
        SeededRandom random = RNGManager.Instance.Generators.worldgen;

        int totalWeight = 0;
        foreach (var structure in structures)
            totalWeight += structure.weight;

        int randomWeight = random.Range(0, totalWeight);
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
        SeededRandom random = RNGManager.Instance.Generators.loot;

        WeightedLootPool chosenPool = default;

        int randomWeight = random.Range(0, totalPoolWeight);
        foreach (var pool in lootTable)
        {
            randomWeight -= pool.weight;
            if (randomWeight < 0)
            {
                chosenPool = pool;
                break;
            }
        }

        if (chosenPool.loot.Length == 0)
        {
            Debug.LogWarning("Empty loot pool");
            return ItemManager.Instance.InvItemAir;
        }

        return RollItem(chosenPool.loot);
    }

    public static InvItem RollItem(WeightedItem[] loot)
    {
        SeededRandom random = RNGManager.Instance.Generators.loot;

        int totalItemWeight = 0;
        foreach (var item in loot)
            totalItemWeight += item.weight;

        int randomWeight = random.Range(0, totalItemWeight);
        foreach (var weightedItem in loot)
        {
            randomWeight -= weightedItem.weight;
            if (randomWeight < 0)
                return weightedItem.item.Roll();
        }

        return ItemManager.Instance.InvItemAir;
    }
}
