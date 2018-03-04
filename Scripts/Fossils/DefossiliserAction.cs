using System.Collections.Generic;

public class DefossiliserAction
{
    public readonly ItemID inItemID;
    public readonly int inItemCount;

    public readonly ItemID outItemID;
    public readonly int outItemCount;

    public float ProcessingTime { get; private set; }

    public DefossiliserAction(ItemID inItemID, ItemID outItemID, float processingTime = 10, int inItemCount=1, int outItemCount=1)
    {
        this.inItemID = inItemID;
        this.inItemCount = inItemCount;
        ProcessingTime = processingTime;
        this.outItemID = outItemID;
        this.outItemCount = outItemCount;
    }

    public ItemStack Process(Inventory inv)
    {
        if (CanBeDoneWith(inv))
        {
            int remaining = inItemCount;
            while (remaining > 0)
            {
                int i = inv.TryGetStackIndex(inItemID).Value;
                int count = inv.GetItemStack(i).Count;
                if (remaining >= count)
                {
                    inv.RemoveItemStack(i);
                    remaining -= count;
                }
                else
                {
                    inv.GetItemStack(i).ChangeQuantity(-remaining);
                    remaining -= remaining;
                }
            }
            return new ItemStack(ItemStorage.Instance[outItemID], outItemCount);
        }
        else
        {
            return null;
        }
    }

    public bool CanBeDoneWith(Inventory inv)
    {
        return inv.ItemCount(inItemID) >= inItemCount;
    }

    public override bool Equals(object other)
    {
        return other is DefossiliserAction da && 
            this.inItemID == da.inItemID && this.inItemCount == da.inItemCount && 
            this.outItemID == da.outItemID && this.outItemCount == da.outItemCount;
    }

    public override int GetHashCode()
    {
        return new List<object> { inItemID, inItemCount, outItemID, outItemCount }.GetHashCode();
    }
}