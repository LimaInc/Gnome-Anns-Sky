using Godot;
using System;

public class GUIButton : Control
{
    public override void _Input(InputEvent ev)
    {
        if (ev.IsPressed())
        {
            GD.Print(ev);
        }
    }
}