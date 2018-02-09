using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

// a ExoWorld object represents the non-player environment of the game
// feel free to add your world elements
public class ExoWorld : Node
{
    public WorldEnvironment WorldEnv { get; private set; }
    public Atmosphere Atmosphere { get; private set; }
    public BacterialState Bacteria { get; private set; } 

    // animals
    // plantlife
    // terrain

    public ExoWorld()
    {
        GD.Print("ExoWorld()");
        this.Atmosphere = new Atmosphere(new StaticAtmosphereComponent(), new SimpleGraphicsAtmosphereComponent());
        this.Bacteria = new BacterialState(new StaticBacterialStateComponent());
    }

    public override void _Ready()
    {
        WorldEnv = GetNode(Game.WORLD_ENVIRO_PATH) as WorldEnvironment;

    }

    public override void _Process(float delta)
    {
        Atmosphere.Update(delta, this);
        Bacteria.Update(delta, this);
    }
}