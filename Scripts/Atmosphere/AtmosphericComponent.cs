using Godot;

public abstract class AtmosphericComponent : Node
{
    protected static Atmosphere atm;

    public override void _Ready()
    {
        base._Ready();
        atm = GetNode(Game.ATMOSPHERE_PATH) as Atmosphere;
    }
}