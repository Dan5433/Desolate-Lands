using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CustomClasses
{
    [Serializable]
    public class WeightedTile
    {
        [SerializeField] int weight;
        [SerializeField] int tileId;
        public int Weight { get { return weight; } }
        public int TileId { get { return tileId; } }
    }

    [Serializable]
    public class WeightedItem
    {
        [SerializeField] int weight;
        [SerializeField] Item item;
        public int Weight { get { return weight; } }
        public Item Item { get { return item; } }
    }
    [Serializable]
    public class WeightedStructure
    {
        [SerializeField] int weight;
        [SerializeField] Tilemap structure;
        public int Weight { get { return weight; } }
        public Tilemap Structure { get { return structure; } }
    }

    [Serializable]
    public class WeightedSprite
    {
        [SerializeField] int weight;
        [SerializeField] Sprite sprite;
        public int Weight { get { return weight; } }
        public Sprite Sprite { get { return sprite; } }
    }
}
