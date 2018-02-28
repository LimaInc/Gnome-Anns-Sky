using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

// a ExoWorld object represents the non-player environment of the game
// feel free to add your world elements
public class PlanetEnvironment : WorldEnvironment
{
    public Atmosphere Atmosphere { get; private set; }
    public BacterialState Bacteria { get; private set; } 

    // animals
    // plantlife
    // terrain

    public override void _Ready()
    {
        Atmosphere = GetNode(Game.ATMOSPHERE_PATH) as Atmosphere;
        Bacteria = GetNode(Game.BACTERIAL_STATE_PATH) as BacterialState;
        this.Atmosphere.Init(new GasEscapingAC(), new ColorMixingAC());
        this.Bacteria.Init(new AsymptoticGrowthBSC(), new ProductionBSC());
    }
}