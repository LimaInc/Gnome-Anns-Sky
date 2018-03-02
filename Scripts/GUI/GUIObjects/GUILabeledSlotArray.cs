using System;
using Godot;

public class GUILabeledSlotArray : GUIInventorySlotArray
{
    private Label2D header;

    public Vector2 LabelShift { get; private set; }

    public GUILabeledSlotArray(GUIInventorySlot exchangeSlot, Item.ItemType type, String labelText,
        IntVector2 size, Vector2 slotSpacing, Vector2 labelShift)
        : base(exchangeSlot, type, size, slotSpacing)
    {
        this.LabelShift = labelShift;

        header = new Label2D
        {
            Text = labelText
        };
        this.AddChild(header);
    }

    public override void HandleResize()
    {
        base.HandleResize();
        PositionHeader();
    }

    private void PositionHeader()
    {
        header.SetPosition(this.LabelShift);
    }

    public void SetSize(Vector2 slotSpacing, Vector2 labelShift)
    {
        this.LabelShift = labelShift;
        base.SetSize(slotSpacing);
    }
}
