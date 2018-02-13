using System;
using Godot;

public class ItemStack
{
    public Item item;
    public int count;

    public ItemStack(Item item, int count)
    {
        this.item = item;
        this.count = count;
    }

    public void AddToQuantity(int c)
    {
        this.count += c;
    }

    public Item GetItem()
    {
        return item;
    }

    public int GetCount()
    {
        return count;
    }

    public void SubtractCount(int n)
    {
        this.count -= n;
    }
}