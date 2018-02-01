using System;
using Godot;

public class InventoryGUI : Node
{
    private GUIButton btn;

    public InventoryGUI()
    {
        btn = new GUIButton();
        btn.SetSize(new Vector2(10.0f, 10.0f));
        this.AddChild(btn);
    }
}