using System;
using System.IO;
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
        [SerializeField] protected Item item;
        [SerializeField] protected string name;
        [SerializeField] protected int count;

        public Item ItemObj => item;
        public string Name => name;
        public int Count
        {
            get { return count; }
            set { count = Mathf.Clamp(value, 0, item.MaxCount); }
        }

        public virtual int CountTxt => count;

        public virtual string ExtraInfo() => item.ExtraInfo();

        public virtual void Save(BinaryWriter writer)
        {
            writer.Write(item.Id);
            writer.Write(name);
            writer.Write(count);
        }
        public InvItem(Item item, BinaryReader reader)
        {
            this.item = item;
            name = reader.ReadString();
            count = reader.ReadInt32();
        }

        public InvItem(Item item, string name, int count)
        {
            this.item = item;
            this.name = name;
            this.count = count;
        }

        public virtual InvItem Clone()
        {
            return new InvItem(item,name,count);
        }
    }

    [Serializable]
    public class InvTool : InvItem
    {
        [SerializeField] int durability;
        public int Durability { get { return durability; } set { durability = value; } }
        public override int CountTxt => durability;
        public override void Save(BinaryWriter writer)
        {
            base.Save(writer);
            writer.Write(durability);
        }
        public InvTool(Item item, BinaryReader reader) : base(item, reader)
        {
            durability = reader.ReadInt32();
        }

        public InvTool(Tool tool, string name, int count) : base(tool, name, count) 
        {
            durability = tool.Durability;
        }
        public InvTool(Tool tool, string name, int count, int durability) : base(tool, name, count)
        {
            this.durability = durability;
        }
        public override InvItem Clone()
        {
            return new InvTool(item as Tool, name, count,durability);
        }
    }

}