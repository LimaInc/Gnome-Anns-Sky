using System;
using System.Collections.Generic;

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
        dynamics.Update(delta, world, this);
    }
}