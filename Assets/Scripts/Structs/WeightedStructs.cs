using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CustomClasses
{
    [Serializable]
    public struct WeightedTileById
    {
        public int weight;
        public int tileId;
    }

    [Serializable]
    public struct WeightedItem
    {
        public int weight;
        public LootItem item;
    }

    [Serializable]
    public struct WeightedLootPool
    {
        public int weight;
        public WeightedItem[] loot;
    }

    [Serializable]
    public struct WeightedStructure
    {
        public int weight;
        public Tilemap structure;
    }

    [Serializable]
    public struct WeightedTile
    {
        public int weight;
        public TileBase tile;
    }
}
