using System;
using Godot;

public class Item
{
    public enum Type
    {
        CONSUMABLE,FOSSIL,BLOCK
    }

    public static Item[] items = new Item[256];

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