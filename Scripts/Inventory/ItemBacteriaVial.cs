using System;
using Godot;

class ItemBacteriaVial : Item  
{
    public float Amount { get; private set;  }

    public ItemBacteriaVial(byte id, String name, Texture tex, float bacteriaAmount = 1) : base(id, name, tex, Item.Type.FOSSIL)
    {
        this.Amount = bacteriaAmount;
    }
}
