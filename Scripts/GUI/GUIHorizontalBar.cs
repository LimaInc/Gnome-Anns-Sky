using Godot;
using System;

public class GUIHorizontalBar : GUIVerticalBar
{
    public GUIHorizontalBar(Vector2 pos, float height, Color c) : base(pos, height, c)
    {
        this.SetRotation(Mathf.PI / 2);
    }
}