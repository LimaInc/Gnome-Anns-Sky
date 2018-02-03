using System;
using Godot;

public class GUIObject : Node
{
    private Rect2 rect;
    private Sprite sprite;

    public GUIObject(Rect2 r)
    {
        this.rect = r;
        this.sprite = new Sprite();
        sprite.SetTexture();
        this.AddChild(sprite);
    }

    public override void _Input(InputEvent e)
    {
        if (e is InputEventMouseButton)
        {
            InputEventMouseButton iemb = (InputEventMouseButton) e;
            Vector2 pos = iemb.GetPosition();
            GD.Print(pos);
            if (rect.HasPoint(pos))
            {
                GD.Print("hit");
            }
        }
    }
}