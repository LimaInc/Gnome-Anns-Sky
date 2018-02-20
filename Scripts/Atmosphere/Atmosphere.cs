using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Atmosphere : Node
{
    private static readonly IDictionary<Gas, float> gasGoals = new Dictionary<Gas, float>
    {
        [Gas.OXYGEN] = 0.2f,
        [Gas.NITROGEN] = 0.7f,
        [Gas.CARBON_DIOXIDE] = 0.1f
    };
    private IDictionary<Gas,float> gases;
    private IAtmosphericComponent dynamics;
    private IAtmosphericComponent graphics;
    public float Pressure { get; private set; }

    public Atmosphere(IAtmosphericComponent dynamics, IAtmosphericComponent graphics)
    {
        gases = new Dictionary<Gas,float>();
        foreach (Gas g in Enum.GetValues(typeof(Gas)))
        {
            gases[g] = 0;
        }
        this.dynamics = dynamics;
        this.graphics = graphics;
    }

    public float GetGasAmt(Gas gas) => gases[gas];

    public float UpdatePressure() => Pressure = gases.Values.Sum();

    public IList<Gas> GetGases() => gases.Keys.ToList();

    public void SetGas(Gas gas, float amt)
    {
        gases[gas] = amt;
    }

    public float GetGasProgress(Gas gas)
    {
        return Math.Min(gases[gas]/gasGoals[gas],1);
    }

    public void Update(float delta, ExoWorld world)
    {
        dynamics.Update(delta, world, this);
        graphics.Update(delta, world, this);
    }
}