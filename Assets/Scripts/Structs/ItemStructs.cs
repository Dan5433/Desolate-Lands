using EditorAttributes;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CustomClasses
{
    [Serializable]
    public enum ToolType
    {
        None,
        Axe,
        Pickaxe,
        Shovel
    }

    [Serializable]
    public enum ItemMaterial
    {
        None,
        Wood,
        Stone,
        RustedIron,
        Copper,
        Iron,
        Aluminium
    }

    [Serializable]
    public struct DropItem
    {
        public Item item;
        public Vector2Int minMax;
        public AnimationCurve quantityDistribution;
        public int RandomCount()
        {
            if (minMax.y > item.MaxCount) minMax.y = item.MaxCount;

            float weightedValue = quantityDistribution.Evaluate(Random.value);
            return (int)Mathf.Lerp(weightedValue, minMax.x, minMax.y);
        }

    }
    [Serializable]
    public class InvItem
    {
        [SerializeField] Item item;
        [SerializeField] string name;
        [SerializeField] int count;

        public Item ItemObj { get { return item; } }
        public string Name { get { return name; } }
        public int Count
        {
            get { return count; }
            set { count = Mathf.Clamp(value, 0, item.MaxCount); }
        }


        public InvItem(Item item, string name, int count)
        {
            this.item = item;
            this.name = name;
            this.count = count;
        }

        public InvItem(InvItem original)
        {
            item = original.item;
            name = original.name;
            count = original.count;
        }
    }

}