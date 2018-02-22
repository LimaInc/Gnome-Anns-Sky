using Godot;
using System;
using System.Linq;

class ProductionBSC : IBacterialStateComponent
{
    public void Update(float delta, ExoWorld w, BacterialState bs)
    {
        Atmosphere atm = w.Atmosphere;
        bs.GetBacteriaList().ForEach(b =>
        {
            atm.ChangeGasAmt(b.GasType, delta * b.Amount * b.ProductionRate);
        });
    }
}
