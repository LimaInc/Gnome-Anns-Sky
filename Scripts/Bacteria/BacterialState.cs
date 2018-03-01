using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class BacterialState : Node
{
    private static readonly IDictionary<BacteriumType, Tuple<float,float,float,float>> bInits = 
        new Dictionary<BacteriumType, Tuple<float, float, float, float>>
    {
        [BacteriumType.OXYGEN] = Tuple.Create(0f, 1f, 0.0008f, 0.10f),
        [BacteriumType.NITROGEN] = Tuple.Create(0f, 1f, 0.0003f, 0.15f),
        [BacteriumType.CARBON_DIOXIDE] = Tuple.Create(0f, 1f, 0.0015f, 0.05f)
    };

    private IDictionary<BacteriumType,Bacteria> bacteriaTypes;

    public BacterialState()
    {
        bacteriaTypes = new Dictionary<BacteriumType, Bacteria>();
        foreach (BacteriumType bt in Enum.GetValues(typeof(BacteriumType)))
        {
            bacteriaTypes[bt] = new Bacteria(bt, amt: bInits[bt].Item1, optimal: bInits[bt].Item2,
                growthRate: bInits[bt].Item3, productionRate: bInits[bt].Item4);
        }
    }

    public void AddComponents(params BacterialStateComponent[] components)
    {
        foreach (BacterialStateComponent cmp in components)
        {
            this.AddChild(cmp);
        }
    }

    public Bacteria GetBacteria(BacteriumType type) => bacteriaTypes[type];

    public bool TryGetBacteria(BacteriumType type, out Bacteria bacteria) 
        => bacteriaTypes.TryGetValue(type, out bacteria);

    public List<Bacteria> GetBacteriaList() => bacteriaTypes.Values.ToList();
}