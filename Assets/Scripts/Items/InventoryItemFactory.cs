using CustomClasses;
using System;
using System.IO;

public static class InventoryItemFactory
{
    public static InvItem Create(BinaryReader reader)
    {
        return ItemManager.Instance.Items[reader.ReadInt32()] switch
        {
            Tool tool => new InvTool(tool, reader),
            Item item => new InvItem(item, reader),
            _ => throw new ArgumentException()
        };
    }
    public static InvItem Create(Item item, int count)
    {
        return item switch
        {
            Tool tool => new InvTool(tool, tool.Name, count),
            _ => new InvItem(item, item.Name, count)
        };
    }
    public static InvItem Create(Item item, string name, int count)
    {
        return item switch
        {
            Tool tool => new InvTool(tool, name, count),
            _ => new InvItem(item, name, count)
        };
    }
}
