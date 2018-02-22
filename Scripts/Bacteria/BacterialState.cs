using System;
using System.Collections.Generic;
using System.Linq;

public class BacterialState
{
    private static readonly IDictionary<BacteriumType, Tuple<float,float,float,float>> bInits = 
        new Dictionary<BacteriumType, Tuple<float, float, float, float>>
    {
        [BacteriumType.OXYGEN] = Tuple.Create(0f, 1f, 0.0008f, 0.05f),
        [BacteriumType.NITROGEN] = Tuple.Create(0f, 1f, 0.0003f, 0.05f),
        [BacteriumType.CARBON_DIOXIDE] = Tuple.Create(0f, 1f, 0.002f, 0.05f)
    };

    private IDictionary<BacteriumType,Bacteria> bacteriaTypes;
    private List<IBacterialStateComponent> components;

    public BacterialState(params IBacterialStateComponent[] dynamics)
    {
        bacteriaTypes = new Dictionary<BacteriumType, Bacteria>();
        foreach(BacteriumType bt in Enum.GetValues(typeof(BacteriumType)))
        {
            bacteriaTypes[bt] = new Bacteria(bt, bInits[bt].Item1, bInits[bt].Item2, bInits[bt].Item3, bInits[bt].Item4);
        }
        this.components = new List<IBacterialStateComponent>(dynamics);
    }

    public Bacteria GetBacteria(BacteriumType type) => bacteriaTypes[type];

    public bool TryGetBacteria(BacteriumType type, out Bacteria bacteria) 
        => bacteriaTypes.TryGetValue(type, out bacteria);

    public void Update(float delta, ExoWorld world)
    {
        components.ForEach(c => c.Update(delta, world, this));
    }

    public List<Bacteria> GetBacteriaList() => bacteriaTypes.Values.ToList();
}