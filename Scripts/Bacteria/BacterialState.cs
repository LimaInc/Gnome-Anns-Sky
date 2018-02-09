using System;
using System.Collections.Generic;
using System.Linq;

public class BacterialState
{
    public enum BacteriumType
    {
        OXYGEN, NITROGEN, CARBON_DIOXIDE
    }

    private static readonly IDictionary<BacteriumType, Tuple<float,float>> bacteriaInitis = new Dictionary<BacteriumType, Tuple<float, float>>
    {
        [BacteriumType.OXYGEN] = Tuple.Create(2f,0.003f),
        [BacteriumType.NITROGEN] = Tuple.Create(8f, 0.0005f),
        [BacteriumType.CARBON_DIOXIDE] = Tuple.Create(1f, 0.015f)
    };

    private IDictionary<BacteriumType,Bacteria> bacteriaTypes;
    private IBacterialStateComponent dynamics;

    public BacterialState(IBacterialStateComponent dynamics)
    {
        bacteriaTypes = new Dictionary<BacteriumType, Bacteria>();
        foreach(BacteriumType bt in Enum.GetValues(typeof(BacteriumType)))
        {
            bacteriaTypes[bt] = new Bacteria(0, bacteriaInitis[bt].Item1, bacteriaInitis[bt].Item2);
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

    public List<Bacteria> GetBacteriaList()
    {
        return bacteriaTypes.Values.ToList();
    }
}