using System;
using Godot;

public class ItemConsumable : Item
{
    // TODO: split into superclass and subclass, not all consumables need to replenish a stat
    public float StatValueChange { get; }
    public Player.Stats StatToReplenish { get; }

    public ItemConsumable(ItemID id, String name, Texture tex, Player.Stats stat, float replenishValue) : base(id, name, tex, Item.ItemType.CONSUMABLE)
    {
        StatValueChange = replenishValue;
        StatToReplenish = stat;
    }
}