using Godot;
using System;

public class GUIVerticalBar : GUIObject
{
    public static Texture TEX = ResourceLoader.Load("res://Images/bar.png") as Texture;
    public static float WIDTH = 64;

    private ColorRect cRect;
    private float perc;
    private float height;

    private Color color;

    public GUIVerticalBar(Vector2 pos, float height, Color c)
        : base(pos, new Rect2(new Vector2(), new Vector2(WIDTH,height)), TEX)
    {   
        this.height = height;
        this.color = c;

        cRect = new ColorRect();
        cRect.Color = c;

        this.RemoveChild(this.sprite);
        this.AddChild(cRect);
        this.AddChild(this.sprite);
    }

    public void SetPercentage(float f)
    {
        this.perc = f;
        cRect.SetPosition(rect.Position + new Vector2(0, (1-perc)*height));
        cRect.SetSize(new Vector2(WIDTH, perc * height));
    }
}