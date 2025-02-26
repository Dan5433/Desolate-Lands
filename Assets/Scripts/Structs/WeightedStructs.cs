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
        public Item item;
        public int weight;
        public Vector2Int minMax;
        public AnimationCurve quantityDistribution;
        public InvItem Roll()
        {
            if (item == ItemManager.Instance.Air)
                return ItemManager.Instance.InvItemAir;

            if(minMax.y > item.MaxCount) 
                minMax.y = item.MaxCount;

            float weightedValue = quantityDistribution.Evaluate(GameRandom.Value);
            int count = (int)Mathf.Lerp(minMax.x, minMax.y, weightedValue);

            if(count < 1)
                return ItemManager.Instance.InvItemAir;

            return InventoryItemFactory.Create(item, count);
        }
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
