using Godot;
using System;

public class Inventory
{
    public static int SLOT_COUNT = 40;
    private Item.Type type;
    private byte[] items;

    public Inventory(Item.Type type)
    {
        this.type = type;
        this.items = new byte[256];
    }

    public void AddItem(Item item)
    {
        for (int i = 0; i < SLOT_COUNT; i++)
        {
            if (items[i] == 0)
            {
                items[i] = item.GetID();
                return;
            }
        }
    }

    public Item GetItem(int index)
    {
        return Item.items[items[index]];
    }
}