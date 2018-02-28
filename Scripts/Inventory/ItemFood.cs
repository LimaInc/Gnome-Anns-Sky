using System;
using Godot;

public class ItemFood : Item
{
    public float ReplenishValue { get; }

    public ItemFood(byte id, String name, Texture tex, float replenishValue) : base(id, name, tex, Item.Type.CONSUMABLE) 
    {
        this.ReplenishValue = replenishValue;
    }
}