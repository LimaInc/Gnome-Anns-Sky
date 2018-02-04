using Godot;
using System;

public class Inventory
{
    private Player player;

    public static int SLOT_COUNT = 40;
    private Item.Type type;
    private byte[] items;

    public Inventory(Player p, Item.Type type)
    {
        this.player = p;
        this.type = type;
        this.items = new byte[256];
    }

    public void RemoveItem(int ind)
    {
        items[ind] = 0;
    }

    //Add item at specific index, used in GUI
    public void AddItem(Item item, int ind)
    {
        if (items[ind] != 0) //hopefully, this shouldn't happen...
            return;

        items[ind] = item.GetID();
    }

    public void AddItem(Item item)
    {
        if (item.GetType() != type)
        {
            GD.Print("Something just tried to add a " + item + " to a " + this.type + " inventory!");
        }
        
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