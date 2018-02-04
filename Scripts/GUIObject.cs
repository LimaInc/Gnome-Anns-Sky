using System;
using Godot;

public class GUIObject : Node
{
    protected Rect2 rect;
    protected Sprite sprite;

    public GUIObject(Rect2 r, Texture t)
    {
        this.rect = r;
        this.sprite = new Sprite();
        sprite.SetTexture(t);
        sprite.SetPosition(r.Position);
        sprite.SetScale(new Vector2(r.Size.x / t.GetSize().x, r.Size.y / t.GetSize().y));
        this.AddChild(sprite);
    }

    //For GUI objects that need manual scalling
    public GUIObject(Rect2 r, Texture t, Vector2 scale)
    {
        this.rect = r;
        this.sprite = new Sprite();
        sprite.SetTexture(t);
        sprite.SetPosition(r.Position);
        sprite.SetScale(scale);
        this.AddChild(sprite);
    }

    public override void _Input(InputEvent e)
    {
        if (e is InputEventMouseButton)
        {
            InputEventMouseButton iemb = (InputEventMouseButton) e;
            Vector2 pos = iemb.GetPosition();
            // GD.Print(pos);
            if (rect.HasPoint(pos))
            {
                GD.Print("hit");
            }
        }
    }
}