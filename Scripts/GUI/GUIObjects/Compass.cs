using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

class Compass : GUIObject
{
    public static Texture COMPASS_ARROW = ResourceLoader.Load(Game.GUI_TEXTURE_PATH + "greenArrow.png") as Texture;

    private readonly Vector2 northPole;

    public Compass(Vector2 pos, Vector2 size, Vector2 northPole_) : base(pos, size, COMPASS_ARROW)
    {
        northPole = northPole_;
    }

    public override void _Process(float delta)
    {
        base._Process(delta);
        // TODO: track position of the North Pole and rotate accordingly
    }
}
