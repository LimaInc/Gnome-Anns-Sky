using System;
using System.Collections.Generic;
using Godot;

class ItemBacteriaVial : Item  
{
    private const float DEFAULT_VIAL_AMT = 0.01f;
    private static readonly IDictionary<ItemID, BacteriumType> bacteriumVials = 
        new Dictionary<ItemID, BacteriumType>
    {
        [ItemID.OXYGEN_BACTERIA_VIAL] = BacteriumType.OXYGEN,
        [ItemID.NITROGEN_BACTERIA_VIAL] = BacteriumType.NITROGEN,
        [ItemID.CARBON_DIOXIDE_BACTERIA_VIAL] = BacteriumType.CARBON_DIOXIDE,
    };

    public float Amount { get; private set; }
    public BacteriumType BType { get; private set; }

    public ItemBacteriaVial(ItemID id, String name, Texture tex, float bacteriaAmount = DEFAULT_VIAL_AMT) 
        : base(id, name, tex, Item.ItemType.PROCESSED)
    {
        Amount = bacteriaAmount;
        BType = bacteriumVials[Id];
    }
}
