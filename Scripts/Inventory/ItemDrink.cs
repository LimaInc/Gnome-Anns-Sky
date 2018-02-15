using System;
using Godot;

public class ItemDrink : Item
{
    public float ReplenishValue { get; }

    public ItemDrink(byte id, String name, Texture tex, float replenishValue) : base(id, name, tex, Item.Type.CONSUMABLE)
    {
        this.ReplenishValue = replenishValue;
    }
}