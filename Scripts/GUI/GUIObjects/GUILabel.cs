using Godot;
using System;

public class GUILabel : GUIElement
{
    private Label label;

    public GUILabel(Func<bool> shouldShow = null) : base(shouldShow)
    {
        label = new Label();
        AddChild(label);
    }

    public string Text { get => label.Text; set => label.Text = value; }
}