using System;
using Godot;

public class InventoryGUI : Node
{
    private GuiInventorySlot test;

    public InventoryGUI()
    {
        test = new GuiInventorySlot(new Rect2(0.0f, 0.0f, 10.0f, 10.0f));
        this.AddChild(test);
    }
}