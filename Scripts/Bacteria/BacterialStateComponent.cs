using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public abstract class BacterialStateComponent : Node
{
    protected static BacterialState bs;

    public override void _Ready()
    {
        base._Ready();
        bs = GetNode(Game.BACTERIAL_STATE_PATH) as BacterialState;
    }
}