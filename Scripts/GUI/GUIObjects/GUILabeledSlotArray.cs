using System;
using Godot;

public class GUILabeledSlotArray : GUIInventorySlotArray
{
    private GUILabel header;

    public Vector2 LabelShift { get; private set; }

    public GUILabeledSlotArray(GUIInventorySlot exchangeSlot, Item.ItemType type, String labelText,
        IntVector2 size, Vector2 slotSpacing, Vector2 labelShift, 
        Func<ItemStack,bool> quickMove = null, Action invUpdate = null, Func<bool> shouldShow = null)
        : base(exchangeSlot, type, size, slotSpacing, quickMove, invUpdate, shouldShow)
    {
        LabelShift = labelShift;

        header = new GUILabel
        {
            Text = labelText
        };
        AddChild(header);
    }

    public override void HandleResize()
    {
        base.HandleResize();
        PositionHeader();
    }

    private void PositionHeader()
    {
        header.SetPosition(LabelShift);
    }

    public void SetSize(Vector2 slotSpacing, Vector2 labelShift)
    {
        LabelShift = labelShift;
        SetSize(slotSpacing);
    }
}
