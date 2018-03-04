using System;
using Godot;

class GUICompass : GUIObject
{
    public const string COMPASS_ARROW = "greenArrow";

    private readonly Vector2 northPole;
    private readonly Func<Vector2> posSupplier;
    private readonly Func<Vector2> viewDirSupplier;

    public GUICompass(Vector2 pos, Vector2 size, Vector2 northPole_, 
        Func<Vector2> posSupplier_, Func<Vector2> viewDirSuppier_, Func<bool> shouldShow = null) : 
            base(pos, size, Game.guiResourceLoader.GetResource(COMPASS_ARROW) as Texture, BaseScale(size), shouldShow)
    {
        northPole = northPole_;
        posSupplier = posSupplier_;
        viewDirSupplier = viewDirSuppier_;

        if (size.x > size.y)
        {
            Scale = new Vector2(1, size.y / size.x);
        }
        else
        {
            Scale = new Vector2(size.x / size.y, 1);
        }
    }

    private static Vector2 BaseScale(Vector2 size)
    {
        Texture tex = Game.guiResourceLoader.GetResource(COMPASS_ARROW) as Texture;
        float baseScale = Mathf.Max(size.x, size.y) / tex.GetSize().Length();
        return new Vector2(baseScale, baseScale);
    }

    public override void _Process(float delta)
    {
        Vector2 toPole = northPole - posSupplier();
        sprite.Rotation = viewDirSupplier().AngleTo(toPole);
    }
}
