using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CustomClasses
{
    [Serializable]
    public struct WeightedTileById
    {
        [SerializeField] int weight;
        [SerializeField] int tileId;
        public int Weight { get { return weight; } }
        public int TileId { get { return tileId; } }
    }

    [Serializable]
    public struct WeightedItem
    {
        [SerializeField] int weight;
        [SerializeField] Item item;
        public int Weight { get { return weight; } }
        public Item Item { get { return item; } }
    }
    [Serializable]
    public struct WeightedStructure
    {
        [SerializeField] int weight;
        [SerializeField] Tilemap structure;
        public int Weight { get { return weight; } }
        public Tilemap Structure { get { return structure; } }
    }

    [Serializable]
    public struct WeightedBreakableTile
    {
        [SerializeField] int weight;
        [SerializeField] BreakableTile tile;
        public int Weight { get { return weight; } }
        public BreakableTile Tile { get { return tile; } }
    }
}
