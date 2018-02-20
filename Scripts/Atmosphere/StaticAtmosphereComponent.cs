using System.Collections.Generic;
using System.Linq;
using static Atmosphere;
using static BacterialState;

public class StaticAtmosphereComponent : IAtmosphericComponent
{
    private static readonly Dictionary<BacteriumType, Gas> productionTable = new Dictionary<BacteriumType, Gas>
    {
        [BacteriumType.OXYGEN] = Gas.OXYGEN,
        [BacteriumType.NITROGEN] = Gas.NITROGEN,
        [BacteriumType.CARBON_DIOXIDE] = Gas.CARBON_DIOXIDE
    };

    public void Update(float delta, ExoWorld w, Atmosphere atm)
    {
        IDictionary<Gas,float> gasAmts = new Dictionary<Gas,float>();
        productionTable.Values.ToList().ForEach(gas => { gasAmts[gas] = 0; });
        BacterialState bacteriaState = w.Bacteria;
        foreach(KeyValuePair<BacteriumType, Gas> entry in productionTable)
        {
            if (bacteriaState.TryGetBacteria(entry.Key, out Bacteria bacteria))
            {
                gasAmts[entry.Value] += bacteria.Amount;
            }
        }
        gasAmts.ToList().ForEach(pair => {
            atm.SetGas(pair.Key, pair.Value);
        });
        atm.UpdatePressure();
    }
}