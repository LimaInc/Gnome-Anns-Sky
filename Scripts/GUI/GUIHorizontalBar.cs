using Godot;
using System;

public class GUIHorizontalBar : GUIVerticalBar
{
    public override Vector2 Size { get => new Vector2(height, WIDTH); }

    public GUIHorizontalBar(Vector2 pos, float height, Color c) : base(pos, height, c)
    {
        this.SetRotation(Mathf.PI / 2);
    }
}