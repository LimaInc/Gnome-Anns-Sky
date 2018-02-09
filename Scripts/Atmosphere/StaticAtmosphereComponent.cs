using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using static Atmosphere;
using static BacterialState;

public class StaticAtmosphereComponent : IAtmosphericComponent
{
    private static Dictionary<BacteriumType, Gas> productionTable;

    public StaticAtmosphereComponent()
    {
        productionTable = new Dictionary<BacteriumType, Gas>();
        productionTable[BacteriumType.OXYGEN] = Gas.OXYGEN;
        productionTable[BacteriumType.NITROGEN] = Gas.NITROGEN;
        productionTable[BacteriumType.CARBON_DIOXIDE] = Gas.CARBON_DIOXIDE;
    }

    public void Update(float delta, ExoWorld w, Atmosphere atm)
    {
        double totalGasAmt = 0;
        IDictionary<Gas,double> gasAmts = new Dictionary<Gas,double>();
        productionTable.Values.ToList().ForEach(gas => { gasAmts[gas] = 0; });
        BacterialState bacteriaState = w.Bacteria;
        foreach(KeyValuePair<BacteriumType, Gas> entry in productionTable)
        {
            if (bacteriaState.TryGetBacteria(entry.Key, out Bacteria bacteria))
            {
                gasAmts[entry.Value] += bacteria.Amount;
                totalGasAmt += bacteria.Amount;
            }
        }
        gasAmts.ToList().ForEach(pair => {
            atm.SetGas(pair.Key, pair.Value/totalGasAmt);
        });
    }
}