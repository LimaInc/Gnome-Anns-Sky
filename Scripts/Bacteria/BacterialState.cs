using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class BacterialState
{
    public enum BacteriumType
    {
        OXYGEN, NITROGEN, CARBON_DIOXIDE
    }

    private Dictionary<BacteriumType,Bacteria> bacteriaTypes;
    private IBacterialStateComponent dynamics;

    public BacterialState(IBacterialStateComponent dynamics)
    {
        GD.Print("BacterialState(dynamics)");
        bacteriaTypes = new Dictionary<BacteriumType, Bacteria>();
        foreach(BacteriumType bt in Enum.GetValues(typeof(BacteriumType)))
        {
            bacteriaTypes[bt] = new Bacteria();
        }
        this.dynamics = dynamics;
    }

    public Bacteria GetBacteria(BacteriumType type) => bacteriaTypes[type];

    public bool TryGetBacteria(BacteriumType type, out Bacteria bacteria) 
        => bacteriaTypes.TryGetValue(type, out bacteria);

    public void Update(float delta, ExoWorld world)
    {
        GD.Print("BacterialState.Update(delta, world)");
        dynamics.Update(delta, world, this);
    }
}