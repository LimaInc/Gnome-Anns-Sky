﻿using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

class GUICompass : GUIObject
{
    public static readonly Texture COMPASS_ARROW = ResourceLoader.Load(Game.GUI_TEXTURE_PATH + "greenArrow.png") as Texture;

    private readonly Vector2 northPole;
    private readonly Func<Vector2> posSupplier;
    private readonly Func<Vector2> viewDirSupplier;

    public GUICompass(Vector2 pos, Vector2 size, Vector2 northPole_, 
        Func<Vector2> posSupplier_, Func<Vector2> viewDirSuppier_) : 
            base(pos, size, COMPASS_ARROW, BaseScale(size))
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
        float baseScale = Mathf.Max(size.x, size.y) / COMPASS_ARROW.GetSize().Length();
        return new Vector2(baseScale, baseScale);
    }

    public override void _Process(float delta)
    {
        base._Process(delta);

        Vector2 toPole = northPole - posSupplier();
        Vector2 viewDir = viewDirSupplier();
        Debug.PrintPlaceOccasionally("toPole: " + toPole);
        Debug.PrintPlaceOccasionallyEnd("viewDir: " + viewDir);
        sprite.Rotation = viewDir.AngleTo(toPole);
    }
}