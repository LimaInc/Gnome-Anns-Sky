using Godot;
using System;

public class GUIElement : Node2D
{
    private Func<bool> shouldShow;

    public GUIElement(Func<bool> shouldShow_ = null)
    {
        shouldShow = shouldShow_;
    }

    public override void _Process(float delta)
    {
        if (shouldShow != null)
        {
            if (shouldShow())
            {
                Show();
            }
            else
            {
                Hide();
            }
        }
    }
}
