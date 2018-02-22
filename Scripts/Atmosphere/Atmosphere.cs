using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Atmosphere : Node
{
    private static readonly float DEFAULT_TEMPERATURE = 300;

    private static readonly IDictionary<Gas, float> gasGoals = new Dictionary<Gas, float>
    {
        [Gas.OXYGEN] = 0.2f,
        [Gas.NITROGEN] = 0.7f,
        [Gas.CARBON_DIOXIDE] = 0.1f
    };
    private IDictionary<Gas,float> gases;
    private List<IAtmosphericComponent> components;

    public float Pressure { get; private set; }
    public float Temperature { get; private set; }

    public Atmosphere(params IAtmosphericComponent[] components)
    {
        Temperature = DEFAULT_TEMPERATURE;
        gases = new Dictionary<Gas,float>();
        foreach (Gas g in Enum.GetValues(typeof(Gas)))
        {
            gases[g] = 0;
        }
        this.components = new List<IAtmosphericComponent>(components);
    }

    public float GetGasAmt(Gas gas) => gases[gas];

    public List<Gas> GetGases() => gases.Keys.ToList();

    public void SetGasAmt(Gas gas, float amt)
    {
        gases[gas] = amt;
    }

    public void ChangeGasAmt(Gas gas, float amt)
    {
        gases[gas] += amt;
    }

    public float GetGasProgress(Gas gas)
    {
        return Math.Min(gases[gas]/gasGoals[gas],1);
    }

    public void Update(float delta, ExoWorld world)
    {
        ValidateState();
        components.ForEach(c => c.Update(delta, world, this));
        ValidateState();
    }

    private void ValidateState()
    {
        UpdatePressure();
    }

    private void UpdatePressure()
    {
        Pressure = gases.Values.Sum();
    }
}