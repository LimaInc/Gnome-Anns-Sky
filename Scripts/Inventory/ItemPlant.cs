using System;
using Godot;

public class ItemPlant : Item
{
    public PlantType PType { get; }

    public ItemPlant(ItemID id, String name, Texture tex, PlantType plantType, byte blockFrom, Item itemTo) : base(id, name, tex, ItemType.PROCESSED)
    {
<<<<<<< HEAD
        this.PlantType = plantType;

        ItemStorage.RegisterBlockItem(blockFrom, itemTo != null ? itemTo : this);
=======
        PType = plantType;
        ItemStorage.RegisterBlockItem(blockFrom, itemTo ?? this);
>>>>>>> a84c7dc6cca15a846594375e2f6a3f7f52852593
    }
}
