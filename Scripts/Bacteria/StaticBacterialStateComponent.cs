using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class StaticBacterialStateComponent : IBacterialStateComponent
{
    public void Update(float delta, ExoWorld w, BacterialState bs)
    {
        bs.TryGetBacteria(BacterialState.BacteriumType.OXYGEN, out Bacteria bOxy);
        bs.TryGetBacteria(BacterialState.BacteriumType.NITROGEN, out Bacteria bNito);
        bs.TryGetBacteria(BacterialState.BacteriumType.CARBON_DIOXIDE, out Bacteria bCO2);
        bOxy.Amount = 5;
        bNito.Amount = 6;
        bCO2.Amount = 7;
    }
}