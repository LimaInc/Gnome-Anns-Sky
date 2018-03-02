using System;
using Godot;

public class ItemBlock : Item
{
    public byte Block { get; }

    public ItemBlock(ItemID id, String name, Texture tex, byte b) : base(id, name, tex, Item.ItemType.BLOCK) 
    {
        Block = b;
        ItemStorage.RegisterBlockItem(b, this);
    }
}