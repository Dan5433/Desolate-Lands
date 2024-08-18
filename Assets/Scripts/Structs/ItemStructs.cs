using System;
using UnityEngine;

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
        [SerializeField] Item item;
        [SerializeField] int minDropCount;
        [SerializeField] int maxDropCount;

        public Item Item { get { return item; } }
        public int MinDropCount { get { return minDropCount; } }
        public int MaxDropCount { get { return maxDropCount; } }

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