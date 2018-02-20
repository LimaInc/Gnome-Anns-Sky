using System;
using Godot;

public class ItemPlant : Item
{
    public PlantType plantType { get; }

    public ItemPlant(byte id, String name, Texture tex, PlantType plantType, byte blockFrom, Item itemTo) : base(id, name, tex, Item.Type.FOSSIL)
    {
        this.plantType = plantType;

        ItemStorage.RegisterBlockItem(blockFrom, itemTo);
    }
}
