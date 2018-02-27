using System;
using System.Collections.Generic;
using Godot;

class ItemBacteriaVial : Item  
{
    private const float DEFAULT_VIAL_AMT = 0.01f;
    private static readonly IDictionary<ItemStorage.ItemID, BacteriumType> bacteriumVials = 
        new Dictionary<ItemStorage.ItemID, BacteriumType>
    {
        [ItemStorage.ItemID.OXYGEN_BACTERIA_VIAL] = BacteriumType.OXYGEN,
        [ItemStorage.ItemID.NITROGEN_BACTERIA_VIAL] = BacteriumType.NITROGEN,
        [ItemStorage.ItemID.CARBON_DIOXIDE_BACTERIA_VIAL] = BacteriumType.CARBON_DIOXIDE,
    };

    public float Amount { get; private set;  }

    public ItemBacteriaVial(byte id, String name, Texture tex, float bacteriaAmount = DEFAULT_VIAL_AMT) 
        : base(id, name, tex, Item.Type.FOSSIL)
    {
        this.Amount = bacteriaAmount;
    }

    public BacteriumType BacteriaType()
    {
        return bacteriumVials[(ItemStorage.ItemID) GetID()];
    }
}
