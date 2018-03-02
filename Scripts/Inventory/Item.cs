using System;
using System.Collections.Generic;
using Godot;

public class Item
{
    [Flags]
    public enum ItemType
    {
        CONSUMABLE = 0x1,
        FOSSIL = 0x2,
        BLOCK = 0x4,
        // should be an OR of all other values
        ANY = CONSUMABLE | FOSSIL | BLOCK
    }

    // checks whether t2 is a superset of types in t1
    public static bool CompatibleWith(ItemType t1, ItemType t2)
    {
        return ((~t1 | t2) & ItemType.ANY) == ItemType.ANY;
    }

    public ItemID Id { get; private set;  }
    private Texture Texture { get; set; }
    public ItemType IType { get; private set; }
    public String Name { get; private set; }

    public bool Stackable { get; set; }

    public Item(ItemID id, String name, Texture tex, ItemType type)
    {
        Id = id;
        Texture = tex;
        IType = type;
        Name = name;
        Stackable = false;
    }

    public Sprite GenerateGUISprite()
    {
        Sprite s = new Sprite();
        s.SetTexture(Texture);
        return s;
    }

    public override string ToString()
    {
        return Name;
    }
}