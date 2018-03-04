using Godot;
using System;

public abstract class GUI : GUIComplexObject
{
    private Node viewportSource;

    private Vector2 oldViewportSize;

    public GUI(Node vdSource)
    {
        viewportSource = vdSource;
    }

    public Vector2 GetViewportDimensions()
    {
        return viewportSource.GetViewport().Size;
    }

    public override void _Process(float delta)
    {
        Vector2 curViewportSize = GetViewportDimensions();

        if (curViewportSize != oldViewportSize)
            HandleResize();

        oldViewportSize = GetViewportDimensions();
    }

    public virtual void HandleClose()
    {
    }

    public virtual void HandleOpen(Node parent)
    {
    }
}