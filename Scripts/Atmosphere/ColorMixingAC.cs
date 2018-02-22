using Godot;
using System;
using System.Collections.Generic;

public class ColorMixingAtmosphereComponent : AtmosphericComponent
{
    private static readonly Tuple<Color, Color> DEFAULT_COLORS = new Tuple<Color, Color>(Colors.BLACK, Colors.DARK_GRAY);
    private static readonly float DEFAULT_COLOR_WEIGHT = WeightFromAmt(0.01f);
    private static readonly IDictionary<Gas, Tuple<Color,Color>> colorTable = new Dictionary<Gas, Tuple<Color, Color>> {
            [Gas.OXYGEN] = new Tuple<Color, Color>(Colors.MAGENTA, Colors.GRAY),
            [Gas.NITROGEN] = new Tuple<Color, Color>(Colors.CYAN, Colors.GRAY),
            [Gas.CARBON_DIOXIDE] = new Tuple<Color, Color>(Colors.YELLOW, Colors.GRAY)
    };

    private bool init = true;

    protected static ProceduralSky sky;

    public override void _Ready()
    {
        base._Ready();
        WorldEnvironment we = GetNode(Game.WORLD_ENVIRO_PATH) as WorldEnvironment;
        Godot.Environment env = we.GetEnvironment();

        env.SetBackground(Godot.Environment.BGMode.Sky);
        sky = env.GetSky() as ProceduralSky;
        sky.SetSkyCurve(0.3f);
        sky.SetGroundCurve(0.3f);
    }

    public override void _Process(float delta)
    {
        Tuple<Color, Color> colors = DEFAULT_COLORS;
        float weightSum = DEFAULT_COLOR_WEIGHT;
        foreach (Gas g in atm.GetGases())
        {
            float gAmt = atm.GetGasAmt(g);
            if (gAmt > 0)
            {
                float gWeight = WeightFromAmt(gAmt);
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
        
        sky.SetSkyTopColor(skyColor);
        sky.SetSkyHorizonColor(skyHorizonColor);
        sky.SetGroundHorizonColor(groundHorizonColor);
        sky.SetGroundBottomColor(groundColor);
    }

    public static float WeightFromAmt(float amt)
    {
        return amt * amt * amt;
    }
}