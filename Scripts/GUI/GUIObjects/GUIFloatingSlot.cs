using System;
using Godot;

public class GUIFloatingSlot : GUIInventorySlot
{
    public GUIFloatingSlot() : base(null, Item.ItemType.ANY, -1, new Vector2())
    {
        this.sprite.SetTexture(new ImageTexture());
    }

    public override void OnLeftPress()
    {
    }

    public override void OnHover()
    {
    }

    public override void OnHoverOff()
    {
    }
}