using Godot;
using System;

public class Inventory
{
    public static int SLOT_COUNT = 40;

    private byte[] consumables;
    private byte[] fossils;
    private byte[] blocks;

    public Inventory()
    {
        consumables = new byte[SLOT_COUNT];
        fossils = new byte[SLOT_COUNT];
        blocks = new byte[SLOT_COUNT];

        AddItem(Item.itemBlock);
    }

    public void AddItem(Item item)
    {
        if (item.GetType() == Item.Type.CONSUMABLE)
        {
            AddConsumable(item);
        } if (item.GetType() == Item.Type.FOSSIL)
        {
            AddFossil(item);
        } if (item.GetType() == Item.Type.BLOCK)
        {
            AddBlock(item);
        }
    }

    public Item getConsumable(int i)
    {
        return Item.items[consumables[i]];
    }

    public Item getFossil(int i)
    {
        return Item.items[fossils[i]];
    }

    public Item getBlock(int i)
    {
        return Item.items[blocks[i]];
    }

    private void AddConsumable(Item item)
    {
        for (int i = 0; i < SLOT_COUNT; i++)
        {
            if (consumables[i] == 0)
            {
                consumables[i] = item.GetID();
                return;
            }
        }
    }

    private void AddFossil(Item item)
    {
        for (int i = 0; i < SLOT_COUNT; i++)
        {
            if (fossils[i] == 0)
            {
                fossils[i] = item.GetID();
                return;
            }
        }
    }

    private void AddBlock(Item item)
    {
        for (int i = 0; i < SLOT_COUNT; i++)
        {
            if (blocks[i] == 0)
            {
                blocks[i] = item.GetID();
                return;
            }
        }
    }
}