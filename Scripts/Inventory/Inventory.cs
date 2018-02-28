using Godot;
using System;

public class Inventory
{
    public readonly int size;
    private Item.Type type;
    private ItemStack[] stacks;

    public Inventory(Item.Type type, int size)
    {
        this.type = type;
        this.size = size;
        this.stacks = new ItemStack[this.size];
    }

    public void RemoveItemStack(int ind)
    {
        stacks[ind] = null;
    }

    // Add item at specific index, used in GUI
    // Please be careful when using this function in other contexts
    // Returns true if place successful, otherwise false
    public bool AddItemStack(ItemStack item, int ind)
    {
        if (stacks[ind] != null) // hopefully, this shouldn't happen...
            return false;

        stacks[ind] = item;
        return true;
    }

    public void AddItemStack(ItemStack i)
    {
        AddItem(i.GetItem(), i.GetCount());
    }

    public void AddItem(Item item, int cnt)
    {
        if (item.GetType() != type)
        {
            GD.Print("Something just tried to add a " + item.GetName() + " to a " + this.type + " inventory!");
            return;
        }
        

        //GD.Print(item.GetName() + " contains : " + Contains(item) + " stack : " + item.IsStackable());
        if (!item.IsStackable())
        {
            for (int j = 0; j < cnt; j++)
            {
                for (int i = 0; i < this.size; i++)
                {
                    if (stacks[i] == null)
                    {
                        stacks[i] = new ItemStack(item, 1);
                        break;
                    }
                }
                // endOfLoop : {}
            }
            return;
        }
        else
        {
            if (Contains(item))
            {
                for (int i = 0; i < this.size; i++)
                {
                    if (stacks[i] == null) continue;

                    if (stacks[i].GetItem().GetID() == item.GetID())
                    {
                        stacks[i].AddToQuantity(cnt);
                        return;
                    }
                }
            }
            else 
            {
                for (int i = 0; i < this.size; i++)
                {
                    if (stacks[i] == null)
                    {
                        stacks[i] = new ItemStack(item, cnt);
                        return;
                    }
                }
            }
        }
    }

    public bool Contains(Item item)
    {
        for (int i = 0; i < this.size; i++)
        {
            if (stacks[i] == null) continue;

            if (stacks[i].GetItem().GetID() == item.GetID())
                return true;
        }
        return false;
    }

    public ItemStack GetItemStack(int index)
    {
        return stacks[index];
    }
}