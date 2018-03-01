using System;
using Godot;

public class Item
{
    [Flags]
    public enum Type
    {
        CONSUMABLE = 0x1,
        FOSSIL = 0x2,
        BLOCK = 0x4,
        // should be an OR of all other values
        ANY = CONSUMABLE | FOSSIL | BLOCK
    }

    // checks whether t2 is a superset of types in t1
    public static bool CompatibleWith(Type t1, Type t2)
    {
        return ((~t1 | t2) & Type.ANY) == Type.ANY;
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

    public override string ToString()
    {
        return GetName();
    }
}