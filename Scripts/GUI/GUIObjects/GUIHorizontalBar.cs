using Godot;
using System;

public class GUIHorizontalBar : GUIVerticalBar
{
    public override Vector2 Size { get => new Vector2(height, WIDTH); }

    public GUIHorizontalBar(Vector2 pos, float height, Color c, Func<float> percSupplier = null, Func<bool> shouldShow = null) : 
        base(pos, height, c, percSupplier, shouldShow)
    {
        Rotation = Mathf.PI / 2;
    }
}