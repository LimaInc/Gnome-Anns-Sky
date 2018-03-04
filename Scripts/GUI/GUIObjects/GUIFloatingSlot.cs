using System;
using Godot;

public class GUIFloatingSlot : GUIInventorySlot
{
    public GUIFloatingSlot(Func<bool> shouldShow = null) : base(null, Item.ItemType.ANY, -1, new Vector2(), shouldShow: shouldShow)
    {
        sprite.SetTexture(new ImageTexture());
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