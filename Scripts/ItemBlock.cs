using System;
using Godot;

public class ItemBlock : Item
{
    private static Texture TEX = ResourceLoader.Load("res://Images/itemBlock.png") as Texture;

    public ItemBlock(byte id) : base(id, TEX, Item.Type.BLOCK) { }
}