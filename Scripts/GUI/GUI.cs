using Godot;
using System;

public abstract class GUI : Node
{
    private Node viewportSource;

    private Vector2 oldViewportSize;

    public GUI(Node vdSource)
    {
        this.viewportSource = vdSource;
    }

    public Vector2 GetViewportDimensions()
    {
        return this.viewportSource.GetViewport().GetSize();
    }

    public abstract void HandleResize();

    public override void _Process(float delta)
    {
        Vector2 curViewportSize = GetViewportDimensions();

        if (curViewportSize != oldViewportSize)
            HandleResize();

        oldViewportSize = GetViewportDimensions();
    }
}