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
    private String name;

    private bool stackable = false;

    public Item(byte id, String name, Texture tex, Type type)
    {
        this.id = id;
        this.texture = tex;
        this.type = type;
        this.name = name;

        items[id] = this;
    }

    public Sprite GenerateGUISprite()
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

    public Item SetStackable(bool s)
    {
        this.stackable = s;
        return this;
    }

    public bool IsStackable()
    {
        return this.stackable;
    }

    public String GetName()
    {
        return name;
    }
}