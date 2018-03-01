public class DefossiliserAction
{
    public readonly Item inItem;
    public readonly int inItemCount;

    public readonly Item outItem;
    public readonly int outItemCount;

    public float ProcessingTime { get; private set; }

    public DefossiliserAction(Item inItem, Item outItem, float processingTime = 10, int inItemCount=1, int outItemCount=1)
    {
        this.inItem = inItem;
        this.inItemCount = inItemCount;
        this.ProcessingTime = processingTime;
        this.outItem = outItem;
        this.outItemCount = outItemCount;
    }

    public ItemStack Process(Inventory inv)
    {
        if (CanBeDoneWith(inv))
        {
            int remaining = inItemCount;
            while (remaining > 0)
            {
                int i = inv.TryGetStackIndex(inItem).Value;
                int count = inv.GetItemStack(i).GetCount();
                if (remaining >= count)
                {
                    inv.RemoveItemStack(i);
                    remaining -= count;
                }
                else
                {
                    inv.GetItemStack(i).AddToQuantity(-remaining);
                    remaining -= remaining;
                }
            }
            return new ItemStack(outItem, outItemCount);
        }
        else
        {
            return null;
        }
    }

    public bool CanBeDoneWith(Inventory inv)
    {
        return inv.ItemCount(inItem) >= inItemCount;
    }
}