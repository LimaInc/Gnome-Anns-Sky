using Godot;
using System;

public class Inventory
{
    public readonly int size;
    private Item.ItemType type;
    private ItemStack[] stacks;

    public Inventory(Item.ItemType type, int size)
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
        return TryAddItem(i.Item.Id, i.Count);
    }

    public bool CanAdd(ItemID itemID, int cnt)
    {
        Item item = ItemStorage.Instance[itemID];
        if (!Item.CompatibleWith(item.IType, type))
        {
            GD.Print("Something just tried to add a " + item.Name + " to a " + this.type + " inventory!");
            return false;
        }

        if (!item.Stackable)
        {
            return CountEmptyStacks() >= cnt;
        }
        else
        {
            for (int i = 0; i < this.size; i++)
            {
                if (stacks[i] == null || stacks[i].Item.Id == item.Id)
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

    public bool TryAddItem(ItemID itemID, int cnt)
    {
        Item item = ItemStorage.Instance[itemID];
        if (!CanAdd(itemID, cnt))
        {
            return false;
        }
        else
        {

            if (!item.Stackable)
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
                int? index = TryGetStackIndex(itemID);
                if (index.HasValue)
                {
                    stacks[index.Value].ChangeQuantity(cnt);
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
        throw new Exception("This should never have happened, trying to add "+cnt+" of "+item+
            " to a full inventory, even though the CanAdd function returns "+CanAdd(itemID, cnt));
    }

    public int? TryGetStackIndex(ItemID itemID)
    {
        for (int i = 0; i < this.size; i++)
        {
            if (stacks[i]?.Item.Id == itemID)
                return i;
        }
        return null;
    }

    public int ItemCount(ItemID itemID)
    {
        int count = 0;
        for (int i = 0; i < this.size; i++)
        {
            if (stacks[i]?.Item.Id == itemID)
                count += stacks[i].Count;
        }
        return count;
    }

    public ItemStack GetItemStack(int index)
    {
        return stacks[index];
    }
}