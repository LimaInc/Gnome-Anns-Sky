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
    public bool TryAddItemStack(ItemStack item, int ind)
    {
        if (stacks[ind] != null) // hopefully, this shouldn't happen...
            return false;

        stacks[ind] = item;
        return true;
    }

    public bool TryAddItemStack(ItemStack i)
    {
        return TryAddItem(i.GetItem(), i.GetCount());
    }

    public bool CanAdd(Item item, int cnt)
    {
        if (!Item.CompatibleWith(item.GetType(), type))
        {
            GD.Print("Something just tried to add a " + item.GetName() + " to a " + this.type + " inventory!");
            return false;
        }

        if (!item.IsStackable())
        {
            return CountEmptyStacks() >= cnt;
        }
        else
        {
            for (int i = 0; i < this.size; i++)
            {
                if (stacks[i] == null || stacks[i].GetItem().GetType() == item.GetType())
                {
                    return true;
                }
            }
            return false;
        }
    }

    private int CountEmptyStacks()
    {
        int res = 0;
        for(int i = 0; i < this.size; i++)
        {
            if (this.stacks[i] == null)
            {
                res++;
            }
        }
        return res;
    }

    public bool TryAddItem(Item item, int cnt)
    {
        if (!CanAdd(item, cnt))
        {
            return false;
        }
        else
        {

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
                }
                return true;
            }
            else
            {
                int? index = TryGetStackIndex(item);
                if (index.HasValue)
                {
                    stacks[index.Value].AddToQuantity(cnt);
                    return true;
                }
                else
                {
                    for (int i = 0; i < this.size; i++)
                    {
                        if (stacks[i] == null)
                        {
                            stacks[i] = new ItemStack(item, cnt);
                            return true;
                        }
                    }
                }
            }
        }
        throw new Exception("This should never have happened");
    }

    public int? TryGetStackIndex(Item item)
    {
        for (int i = 0; i < this.size; i++)
        {
            if (stacks[i]?.GetItem()?.GetID() == item.GetID())
                return i;
        }
        return null;
    }

    public int ItemCount(Item item)
    {
        int count = 0;
        for (int i = 0; i < this.size; i++)
        {
            if (stacks[i]?.GetItem()?.GetID() == item.GetID())
                count += stacks[i].GetCount();
        }
        return count;
    }

    public ItemStack GetItemStack(int index)
    {
        return stacks[index];
    }
}