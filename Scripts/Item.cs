using System;
using Godot;

public class Item
{
    public enum Type
    {
        CONSUMABLE,FOSSIL,BLOCK
    }

    public static Item[] items = new Item[256];

    private static Texture BLOCK_TEX = ResourceLoader.Load("res://Images/itemBlock.png") as Texture;
    public static Item block = new Item(1, BLOCK_TEX, Item.Type.BLOCK);

    private static Texture FOSSIL_TEX = ResourceLoader.Load("res://Images/itemFossil.png") as Texture;
    public static Item fossil = new Item(2, FOSSIL_TEX, Item.Type.FOSSIL);

    private static Texture CHOCOLATE_TEX = ResourceLoader.Load("res://Images/itemChocolate.png") as Texture;
    public static Item chocoloate = new Item(3, CHOCOLATE_TEX, Item.Type.CONSUMABLE);

    private byte id;
    private Texture texture;
    private Type type;

    public Item(byte id, Texture tex, Type type)
    {
        this.id = id;
        this.texture = tex;
        this.type = type;

        items[id] = this;
    }

    public Sprite generateGUISprite()
    {
        Sprite s = new Sprite();
        s.SetTexture(texture);
        return s;
    }

    public byte GetID()
    {
        return this.id;
    }

    new public Type GetType()
    {
        return this.type;
    }
}