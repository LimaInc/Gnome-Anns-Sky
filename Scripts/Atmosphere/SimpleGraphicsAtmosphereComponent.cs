using Godot;
using System;
using System.Collections.Generic;
using static Atmosphere;

public class SimpleGraphicsAtmosphereComponent : IAtmosphericComponent
{
    private static readonly IDictionary<Gas, Tuple<Color,Color>> colorTable = new Dictionary<Gas, Tuple<Color, Color>> {
            [Gas.OXYGEN] = new Tuple<Color, Color>(Colors.MAGENTA, Colors.GRAY),
            [Gas.NITROGEN] = new Tuple<Color, Color>(Colors.CYAN, Colors.GRAY),
            [Gas.CARBON_DIOXIDE] = new Tuple<Color, Color>(Colors.YELLOW, Colors.GRAY),
            [Gas.VACUUM] = new Tuple<Color, Color>(Colors.BLACK, Colors.DARK_GRAY)
    };

    public void Update(float delta, ExoWorld w, Atmosphere atm)
    {
        Gas gas = Gas.VACUUM;
        double maxAmt = Double.NegativeInfinity;
        foreach(Gas g in atm.GetGases())
        {
            if (atm.GetGasAmt(g) > maxAmt)
            {
                gas = g;
                maxAmt = atm.GetGasAmt(g);
            }
        }
        Tuple<Color,Color> colors = colorTable[gas];
        Color skyColor = colors.Item1;
        Color groundColor = colors.Item2;
        Color skyHorizonColor = skyColor.LinearInterpolate(groundColor, 0.33f);
        Color groundHorizonColor = skyColor.LinearInterpolate(groundColor, 0.67f);

        Godot.Environment env = w.WorldEnv.GetEnvironment();
        env.SetBackground(Godot.Environment.BGMode.Sky);
        ProceduralSky sky = env.GetSky() as ProceduralSky;
        sky.SetSkyTopColor(skyColor);
        sky.SetSkyHorizonColor(skyHorizonColor);
        sky.SetGroundHorizonColor(groundHorizonColor);
        sky.SetGroundBottomColor(groundColor);
    }
}