using System;
using Godot;

public class GUIObject : Node2D
{
    protected Rect2 rect;
    protected Sprite sprite;

    private bool hovered;

    public GUIObject(Vector2 pos, Rect2 r, Texture t)
    {
        this.SetPosition(pos);
        this.rect = r;
        this.sprite = new Sprite();
        if (t != null)
            sprite.SetTexture(t);
        sprite.SetPosition(r.Position + r.Size / 2);
        sprite.SetScale(new Vector2(r.Size.x / t.GetSize().x, r.Size.y / t.GetSize().y));
        this.AddChild(sprite);
    }

    //For GUI objects that need manual scalling
    public GUIObject(Vector2 pos, Rect2 r, Texture t, Vector2 scale)
    {
        this.SetPosition(pos);
        this.rect = r;
        this.sprite = new Sprite();
        sprite.SetTexture(t);
        sprite.SetPosition(r.Position);
        sprite.SetScale(scale);
        this.AddChild(sprite);
    }

    public virtual void OnLeftPress() { }

    public virtual void OnHover() { }

    public virtual void OnHoverOff() { }

    public override void _Input(InputEvent e)
    {
        if (e is InputEventMouseButton iemb)
        {
            Vector2 pos = iemb.GetPosition();
            if (InputUtil.IsLeftPress(iemb) && rect.HasPoint(this.ToLocal(pos)))
            {
                OnLeftPress();
            }
        }

        if (e is InputEventMouseMotion iemm)
        {
            Vector2 pos = iemm.GetPosition();
            if (rect.HasPoint(pos))
            {
                hovered = true;
                OnHover();
            }
            else
            {
                if (hovered)
                {
                    hovered = false;
                    OnHoverOff();
                }
            }
        }
    }
}