using System;
using Godot;

public class DeadGUI : VisibleMouseGUI
{
    private static readonly Color BACKGROUND_COLOR = new Color(0.8f, 0.0f, 0.0f, 0.5f);

    private const float BASE_TEXT_SCALE = 0.8f;

    private float textScale;
    private Sprite youAreDead;
    private ColorRect background;
    float time;

    private const string DEAD_TEX = "youAreDead";
    
    public DeadGUI(Node vs) : base(vs)
    {
        time = 0;

        background = new ColorRect
        {
            Color = BACKGROUND_COLOR
        };
        AddChild(background);

        youAreDead = new Sprite
        {
            Texture = Game.guiResourceLoader.GetResource(DEAD_TEX) as Texture
        };
        AddChild(youAreDead);
    }

    public override void HandleResize()
    {
        background.SetSize(GetViewportDimensions());

        youAreDead.SetPosition(GetViewportDimensions() / 2);
        textScale = BASE_TEXT_SCALE;
    }

    public override void HandleOpen(Node parent)
    {
        base.HandleOpen(parent);
        HandleResize();
        _Process(0);
        Show();
    }

    public override void _Process(float delta)
    {
        time += delta;
        textScale = BASE_TEXT_SCALE + Mathf.Sin(time * 2) * 0.2f;
        youAreDead.Scale = new Vector2(textScale, textScale);
        youAreDead.Rotation = Mathf.Cos(time) * 0.1f;
    }
}