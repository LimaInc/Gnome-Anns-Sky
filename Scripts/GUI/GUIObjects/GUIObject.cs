using System;
using Godot;

public class GUIObject : Node2D
{
    protected Rect2 rect;
    protected Sprite sprite;
    protected readonly bool automaticRescaling;

    private bool hovered;

    public GUIObject(Vector2 pos, Vector2 size, Texture t)
    {
        automaticRescaling = true;
        Position = pos;
        sprite = new Sprite();
        if (t != null)
            sprite.Texture = t;
        Resize(size);
        AddChild(sprite);
    }

    //For GUI objects that need manual scaling
    public GUIObject(Vector2 pos, Vector2 size, Texture t, Vector2 scale)
    {
        automaticRescaling = false;
        Position = pos;
        rect = new Rect2(new Vector2(), size);
        sprite = new Sprite();
        if (t != null)
            sprite.Texture = t;
        sprite.Scale = scale;
        AddChild(sprite);
    }

    public virtual void OnLeftPress() { }

    public virtual void OnHover() { }

    public virtual void OnHoverOff() { }

    public override void _Input(InputEvent e)
    {
        if (e is InputEventMouseButton iemb)
        {
            Vector2 pos = ToLocal(iemb.GetPosition());
            if (InputUtil.IsLeftPress(iemb) && rect.HasPoint(pos))
            {
                OnLeftPress();
            }
        }

        if (e is InputEventMouseMotion iemm)
        {
            Vector2 pos = ToLocal(iemm.GetPosition());
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

    public virtual void Resize(Vector2 newSize)
    {
        if (automaticRescaling)
        {
            rect = new Rect2(new Vector2(), newSize);
            sprite.SetPosition(newSize / 2);
            sprite.SetScale(new Vector2(rect.Size.x / sprite.Texture.GetSize().x, rect.Size.y / sprite.Texture.GetSize().y));
        }
        else
        {
            throw new NotSupportedException();
        }
    }
}