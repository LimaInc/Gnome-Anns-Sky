﻿using Godot;
using System;
using System.Linq;

class AsymptoticallyGrowingBacteria : IBacterialStateComponent
{
    public void Update(float delta, ExoWorld w, BacterialState bs)
    {
        bs.GetBacteriaList().ForEach(b =>
        {
            b.Amount = (float)(b.OptimalAmount + (b.Amount - b.OptimalAmount) * Math.Exp(-b.GrowthRate * delta));
        });
    }
}