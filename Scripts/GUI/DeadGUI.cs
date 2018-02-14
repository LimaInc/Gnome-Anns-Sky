using System;
using Godot;

public class DeadGUI : GUI
{
    private float textScale = 0.8f;
    private Sprite youAreDead;

    private static Texture DEAD_TEX = ResourceLoader.Load("res://Images/youAreDead.png") as Texture;

    public DeadGUI(Node vs) : base(vs) { }

    public override void HandleResize()
    {
        youAreDead = new Sprite();
        youAreDead.SetPosition(this.GetViewportDimensions() / 2.0f);
        youAreDead.SetTexture(DEAD_TEX);
        youAreDead.SetScale(new Vector2(textScale, textScale));
        this.AddChild(youAreDead);
    }

    float time = 0.0f;
    public override void _Process(float delta)
    {
        time += delta;
        textScale = 0.8f + (float) Math.Sin(time * 2.0f) * 0.2f;
        youAreDead.SetScale(new Vector2(textScale, textScale));
        youAreDead.SetRotation((float) Math.Cos(time * 1.0f) * 0.1f);
    }
}