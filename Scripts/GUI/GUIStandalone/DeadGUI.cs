using System;
using Godot;

public class DeadGUI : GUI
{
    private Color bgCol = new Color(0.8f, 0.0f, 0.0f, 0.5f);

    private float textScale = 0.8f;
    private Sprite youAreDead;

    private static Texture DEAD_TEX = ResourceLoader.Load(Game.GUI_TEXTURE_PATH + "youAreDead.png") as Texture;

    public DeadGUI(Node vs) : base(vs) { }

    public override void HandleResize()
    {
        ColorRect cr = new ColorRect
        {
            Color = bgCol
        };
        cr.SetSize(this.GetViewportDimensions());
        this.AddChild(cr);

        youAreDead = new Sprite();
        youAreDead.SetPosition(this.GetViewportDimensions() / 2.0f);
        youAreDead.SetTexture(DEAD_TEX);
        youAreDead.SetScale(new Vector2(textScale, textScale));
        this.AddChild(youAreDead);
    }

    float time = 0.0f;
    public override void _Process(float delta)
    {
        base._Process(delta);

        time += delta;
        textScale = 0.8f + (float) Math.Sin(time * 2.0f) * 0.2f;
        youAreDead.SetScale(new Vector2(textScale, textScale));
        youAreDead.SetRotation((float) Math.Cos(time * 1.0f) * 0.1f);
    }
}