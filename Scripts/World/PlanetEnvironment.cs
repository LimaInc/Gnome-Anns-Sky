using Godot;

// a ExoWorld object represents the non-player environment of the game
// feel free to add your world elements
public class PlanetEnvironment : WorldEnvironment
{
    public override void _Ready()
    {
        Atmosphere atm = GetNode(Game.ATMOSPHERE_PATH) as Atmosphere;
        BacterialState bacteria  = GetNode(Game.BACTERIAL_STATE_PATH) as BacterialState;
        atm.AddComponents(new GasEscapingAC(), new ColorMixingAC());
        bacteria.AddComponents(new AsymptoticGrowthBSC(), new ProductionBSC());
    }
}