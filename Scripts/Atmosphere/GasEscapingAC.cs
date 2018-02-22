using System.Collections.Generic;
using System.Linq;
using Godot;

public class GasEscapingAC : AtmosphericComponent
{
    private static readonly float escapability = 0.2f;

    public static float EscapeVelocity { get; } = 10f;

    public override void _PhysicsProcess(float delta)
    {
        atm.GetGases().ForEach(g =>
        {
            float x = GasMolecule.gasData[g].Mass * EscapeVelocity / atm.Temperature; // on the order of 1
            // float fractionEscaping = escapability * Mathf.Exp(-x);
            float fractionEscaping = escapability / (1 + x); // on the order of escapability / 2
            float initialAmt = atm.GetGasAmt(g);
            float finalAmt = initialAmt * Mathf.Exp(-fractionEscaping * delta);
            atm.SetGasAmt(g, finalAmt);
        });
    }
}