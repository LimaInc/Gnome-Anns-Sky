using Godot;
using System;
using System.Collections.Generic;
using static Atmosphere;

public class ColorMixingAtmosphereComponent : IAtmosphericComponent
{
    private static readonly Tuple<Color, Color> DEFAULT_COLORS = new Tuple<Color, Color>(Colors.BLACK, Colors.DARK_GRAY);
    private static readonly IDictionary<Gas, Tuple<Color,Color>> colorTable = new Dictionary<Gas, Tuple<Color, Color>> {
            [Gas.OXYGEN] = new Tuple<Color, Color>(Colors.MAGENTA, Colors.GRAY),
            [Gas.NITROGEN] = new Tuple<Color, Color>(Colors.CYAN, Colors.GRAY),
            [Gas.CARBON_DIOXIDE] = new Tuple<Color, Color>(Colors.YELLOW, Colors.GRAY)
    };

    private bool init = true;

    public void Update(float delta, ExoWorld w, Atmosphere atm)
    {
        Tuple<Color, Color> colors = DEFAULT_COLORS;
        float weightSum = 0;
        foreach (Gas g in atm.GetGases())
        {
            float gAmt = atm.GetGasAmt(g);
            if (gAmt > 0)
            {
                float gWeight = gAmt * gAmt * gAmt;
                weightSum += gWeight;
                Tuple<Color, Color> gColors = colorTable[g];
                Color newSkyCol = colors.Item1.LinearInterpolate(gColors.Item1, gWeight / weightSum);
                Color newGndsCol = colors.Item2.LinearInterpolate(gColors.Item2, gWeight / weightSum);
                colors = Tuple.Create(newSkyCol, newGndsCol);
            }
        }
        Color skyColor = colors.Item1;
        Color groundColor = colors.Item2;
        Color skyHorizonColor = skyColor.LinearInterpolate(groundColor, 0.5f);
        Color groundHorizonColor = skyColor.LinearInterpolate(groundColor, 0.5f);

        Godot.Environment env = w.WorldEnv.GetEnvironment();
        env.SetBackground(Godot.Environment.BGMode.Sky);
        ProceduralSky sky = env.GetSky() as ProceduralSky;
        if (init) // TODO: place in _Ready() once everything is a Godot Node
        {
            sky.SetSkyCurve(0.3f);
            sky.SetGroundCurve(0.3f);
            init = false;
        }
        sky.SetSkyTopColor(skyColor);
        sky.SetSkyHorizonColor(skyHorizonColor);
        sky.SetGroundHorizonColor(groundHorizonColor);
        sky.SetGroundBottomColor(groundColor);
    }
}