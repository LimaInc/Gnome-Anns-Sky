public class ItemStack
{
    public Item Item { get; private set; }
    public int Count { get; private set; }

    public ItemStack(Item item, int count)
    {
        Item = item;
        Count = count;
    }

    public void ChangeQuantity(int c)
    {
        Count += c;
    }

    public override string ToString()
    {
        return "ItemStack(" + Item + ", " + Count + ")";
    }
}