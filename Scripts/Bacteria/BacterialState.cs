using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class BacterialState : Node
{
    private static readonly IDictionary<BacteriumType, Tuple<float,float,float,float>> bInits = 
        new Dictionary<BacteriumType, Tuple<float, float, float, float>>
    {
        [BacteriumType.OXYGEN] = Tuple.Create(0f, 1f, 0.0008f, 0.05f),
        [BacteriumType.NITROGEN] = Tuple.Create(0f, 1f, 0.0003f, 0.05f),
        [BacteriumType.CARBON_DIOXIDE] = Tuple.Create(0f, 1f, 0.002f, 0.05f)
    };

    private IDictionary<BacteriumType,Bacteria> bacteriaTypes;

    public void Init(params BacterialStateComponent[] components)
    {
        bacteriaTypes = new Dictionary<BacteriumType, Bacteria>();
        foreach(BacteriumType bt in Enum.GetValues(typeof(BacteriumType)))
        {
            bacteriaTypes[bt] = new Bacteria(bt, bInits[bt].Item1, bInits[bt].Item2, bInits[bt].Item3, bInits[bt].Item4);
        }
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