using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

class ItemSpawnEgg : Item
{
    public string Preset { get; }

    public ItemSpawnEgg(ItemID id, string name, Texture tex, string animalPreset) : base(id, name, tex, ItemType.PROCESSED)
    {
        Preset = animalPreset;
    }
}
