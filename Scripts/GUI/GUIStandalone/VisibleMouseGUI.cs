using System;
using Godot;

public abstract class VisibleMouseGUI : GUI
{
    private Input.MouseMode prevMode;

    public VisibleMouseGUI(Node vdSource) : base(vdSource)
    {
    }

    public override void HandleOpen(Node parent)
    {
        prevMode = Input.GetMouseMode();
        Input.SetMouseMode(Input.MouseMode.Visible);
    }

    public override void HandleClose()
    {
        Input.SetMouseMode(prevMode);
    }
}