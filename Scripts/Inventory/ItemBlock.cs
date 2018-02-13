using System;
using Godot;

public class ItemBlock : Item
{
    public byte Block { get; }

    public ItemBlock(byte id, String name, Texture tex, byte b) : base(id, name, tex, Item.Type.BLOCK) 
    {
        this.Block = b;
    }
}