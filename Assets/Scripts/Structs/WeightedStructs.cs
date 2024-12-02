using EditorAttributes;
using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

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

        public int RandomCount()
        {
            if(minMax.y > item.MaxCount) minMax.y = item.MaxCount;

            float weightedValue = quantityDistribution.Evaluate(Random.value);
            return (int)Mathf.Lerp(weightedValue, minMax.x, minMax.y);
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
