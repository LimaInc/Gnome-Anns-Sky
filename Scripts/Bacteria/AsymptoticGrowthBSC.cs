using Godot;
using System;
using System.Linq;

class AsymptoticGrowthBSC : BacterialStateComponent
{
    public override void _PhysicsProcess(float delta)
    {
        bs.GetBacteriaList().ForEach(b =>
        {
            if (b.Amount > 0)
            {
                b.Amount = (float)(b.OptimalAmount + (b.Amount - b.OptimalAmount) * Math.Exp(-b.GrowthRate * delta));
            }
        });
    }
}
