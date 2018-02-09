using Godot;
using System;

public class GUIHorizontalBar : GUIObject
{
    public static Texture TEX = ResourceLoader.Load("res://Images/bar.png") as Texture;
    public static float WIDTH = 64.0f;

    private Sprite top = new Sprite();
    private Sprite bottom = new Sprite();

    private ColorRect cRect;
    private float perc;
    private float height;

    private Color color;

    public GUIHorizontalBar(Vector2 pos, float height, Color c) : base(new Rect2(pos, new Vector2(WIDTH,height)), TEX, new Vector2(WIDTH / 16.0f, height / 16.0f))
    {   
        this.height = height;
        this.color = c;

        this.sprite.SetRegion(true);
        this.sprite.SetRegionRect(new Rect2(0, 16, 16, 16));
        this.sprite.SetRotation((float) Math.PI / 2.0f);

        top.SetTexture(TEX);
        top.SetRegion(true);
        top.SetRegionRect(new Rect2(0, 0, 16, 16));
        top.SetPosition(new Vector2(pos.x + height / 2.0f, pos.y));
        top.SetScale(new Vector2(WIDTH / 16.0f, WIDTH / 16.0f));
        top.SetRotation((float) Math.PI / 2.0f);
        this.AddChild(top);

        bottom.SetTexture(TEX);
        bottom.SetRegion(true);
        bottom.SetRegionRect(new Rect2(0, 32, 16, 16));
        bottom.SetPosition(new Vector2(pos.x - height / 2.0f, pos.y));
        bottom.SetScale(new Vector2(WIDTH / 16.0f, WIDTH / 16.0f));
        bottom.SetRotation((float) Math.PI / 2.0f);
        this.AddChild(bottom);

        cRect = new ColorRect();
        cRect.Color = c;
        this.AddChild(cRect);
    }

    public void SetPercentage(float f)
    {
        this.perc = f;
        //Color rect positions are set from top left, so the position setting is a bit messy
        cRect.SetPosition(rect.Position + new Vector2(-height / 2.0f - 24.0f, -WIDTH / 2.0f + 8.0f));
        cRect.SetSize(new Vector2(perc * (height + 48.0f), WIDTH - 16.0f));
    }
}