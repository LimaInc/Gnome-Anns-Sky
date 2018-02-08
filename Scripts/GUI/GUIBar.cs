using Godot;
using System;

public class GUIBar : GUIObject
{
    public static Texture TEX = ResourceLoader.Load("res://Images/bar.png") as Texture;
    public static float WIDTH = 64.0f;

    private Sprite top = new Sprite();
    private Sprite bottom = new Sprite();

    private ColorRect cRect;
    private float perc;
    private float height;

    public GUIBar(Vector2 pos, float height) : base(new Rect2(pos, new Vector2(WIDTH,height)), TEX, new Vector2(WIDTH / 16.0f, height / 16.0f))
    {   
        this.height = height;

        //Test, delete
        this.perc = 1.0f;

        this.sprite.SetRegion(true);
        this.sprite.SetRegionRect(new Rect2(0, 16, 16, 16));

        top.SetTexture(TEX);
        top.SetRegion(true);
        top.SetRegionRect(new Rect2(0, 0, 16, 16));
        top.SetPosition(new Vector2(pos.x, pos.y - height / 2.0f));
        top.SetScale(new Vector2(WIDTH / 16.0f, WIDTH / 16.0f));
        this.AddChild(top);

        bottom.SetTexture(TEX);
        bottom.SetRegion(true);
        bottom.SetRegionRect(new Rect2(0, 32, 16, 16));
        bottom.SetPosition(new Vector2(pos.x, pos.y + height / 2.0f));
        bottom.SetScale(new Vector2(WIDTH / 16.0f, WIDTH / 16.0f));
        this.AddChild(bottom);

        cRect = new ColorRect();
        //Color rect positions are set from top left
        cRect.SetPosition(pos - new Vector2(WIDTH / 2.0f, height / 2.0f) + new Vector2(8.0f, 8.0f - 32.0f));
        cRect.SetSize(new Vector2(WIDTH - 16.0f, perc * (height + 48.0f)));
        this.AddChild(cRect);
    }

    public void setPercentage(float f)
    {
        this.perc = f;
    }
}