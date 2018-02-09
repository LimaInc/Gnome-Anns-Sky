using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Atmosphere : Node
{
    public enum Gas
    {
        OXYGEN, NITROGEN, CARBON_DIOXIDE,
        VACUUM
    }

    private Dictionary<Gas,Double> gases;
    private IAtmosphericComponent dynamics;
    private IAtmosphericComponent graphics;

    public Atmosphere(IAtmosphericComponent dynamics, IAtmosphericComponent graphics)
    {
        GD.Print("Atmosphere(dynamics, graphics)");
        gases = new Dictionary<Gas,Double>();
        foreach (Gas g in Enum.GetValues(typeof(Gas)))
        {
            gases[g] = 0;
        }
        this.dynamics = dynamics;
        this.graphics = graphics;
    }

    public Double GetGasAmt(Gas gas) => gases[gas];

    public IList<Gas> GetGases() => gases.Keys.ToList();

    public void SetGas(Gas gas, double amt)
    {
        gases[gas] = amt;
    }

    public void Update(float delta, ExoWorld world)
    {
        GD.Print("Atmosphere.Update(delta, world)");
        dynamics.Update(delta, world, this);
        graphics.Update(delta, world, this);
    }
}