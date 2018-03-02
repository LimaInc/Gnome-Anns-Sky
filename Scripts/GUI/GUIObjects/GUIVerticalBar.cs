using Godot;
using System;

public class GUIVerticalBar : GUIObject
{
    public static Texture TEX = ResourceLoader.Load(Game.GUI_TEXTURE_PATH + "bar.png") as Texture;
    public static float WIDTH = 64;

    private ColorRect cRect;
    protected float height;
    private float perc;
    public float Percentage
    {
        get => perc;
        set {
            perc = Mathf.Min(1, Mathf.Max(0, value));
            cRect.SetPosition(rect.Position + new Vector2(0, (1 - perc) * height));
            cRect.SetSize(new Vector2(WIDTH, perc * height));
        }
    }

    private Color color;

    public virtual Vector2 Size { get => new Vector2(WIDTH, height); }

    public GUIVerticalBar(Vector2 pos, float height, Color c)
        : base(pos, new Vector2(WIDTH,height), TEX)
    {   
        this.height = height;
        this.color = c;

        cRect = new ColorRect
        {
            Color = c
        };

        this.RemoveChild(this.sprite);
        this.AddChild(cRect);
        this.AddChild(this.sprite);
    }
}