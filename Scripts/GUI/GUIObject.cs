using System;
using Godot;

public class GUIObject : Node
{
    protected Rect2 rect;
    protected Sprite sprite;

    private bool hovered;

    public GUIObject(Rect2 r, Texture t)
    {
        this.rect = r;
        this.sprite = new Sprite();
        if (t != null)
            sprite.SetTexture(t);
        sprite.SetPosition(r.Position + r.Size / 2.0f);
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

    public virtual void OnClick() { }

    public virtual void OnHover() { }

    public virtual void OnHoverOff() { }

    public override void _Input(InputEvent e)
    {
        if (e is InputEventMouseButton iemb)
        {
            Vector2 pos = iemb.GetPosition();
            if (InputUtil.IsLeftPress(iemb) && rect.HasPoint(pos))
            {
                OnClick();
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