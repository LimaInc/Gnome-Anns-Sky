using System;
using System.Collections.Generic;

class GasMolecule
{
    public static readonly IDictionary<Gas, GasMolecule> gasData = new Dictionary<Gas, GasMolecule>
    {
        [Gas.OXYGEN] = new GasMolecule(Gas.OXYGEN, 32),
        [Gas.NITROGEN] = new GasMolecule(Gas.NITROGEN, 28),
        [Gas.CARBON_DIOXIDE] = new GasMolecule(Gas.CARBON_DIOXIDE, 44, 1)
    };
    public Gas Gas { get; private set; }
    public float Mass { get; private set; }
    public float HeatAbsorbance { get; private set; }

    public GasMolecule(Gas g, float mass, float heatAbsorbance = 0)
    {
        this.Gas = g;
        this.Mass = mass;
        this.HeatAbsorbance = heatAbsorbance;
    }
}
