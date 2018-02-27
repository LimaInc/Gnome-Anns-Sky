public class DefossiliserAction
{
    private Item inItem;
    private int inItemCount;
    private ItemStack outStack;

    public DefossiliserAction(Item inItem, Item outItem, int inItemCount=1, int outItemCount=1)
    {
        this.inItem = inItem;
        this.inItemCount = inItemCount;
        this.outStack = new ItemStack(outItem, outItemCount);
    }

    public ItemStack Process(ItemStack stack)
    {
        if (stack.GetItem() == inItem && stack.GetCount() >= inItemCount)
        {
            stack.SubtractCount(inItemCount);
            return new ItemStack(stack.item, stack.count);
        }
        else
        {
            return null;
        }
    }
}