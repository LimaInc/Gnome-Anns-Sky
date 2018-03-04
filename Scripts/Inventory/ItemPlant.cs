using System;
using Godot;

public class ItemPlant : Item
{
    public PlantType PType { get; }

    public ItemPlant(ItemID id, String name, Texture tex, PlantType plantType, byte blockFrom, Item itemTo) : base(id, name, tex, ItemType.PROCESSED)
    {
        PType = plantType;
        ItemStorage.RegisterBlockItem(blockFrom, itemTo ?? this);
    }
}
