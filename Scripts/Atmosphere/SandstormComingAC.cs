using Godot;
using System;
using System.Collections.Generic;

public class SandstormComingAC : AtmosphericComponent {
	public enum Status
	{
    	DORMANT,
    	WARNING,
    	ACTIVE
	};

	private static readonly Color WARNING_COLOUR_SKY = Colors.SCARLET;
	private static readonly Color WARNING_COLOUR_GROUND = Colors.DARK_RED;

	private static Status status;
	private static float timeAlive;

    protected static ProceduralSky sky;
    protected static DisasterManager dm;

    public override void _Ready()
    {
        base._Ready();
        WorldEnvironment we = GetNode(Game.WORLD_ENVIRO_PATH) as WorldEnvironment;
        Godot.Environment env = we.GetEnvironment();

        env.SetBackground(Godot.Environment.BGMode.Sky);
        sky = env.GetSky() as ProceduralSky;
        sky.SetSkyCurve(0.3f);
        sky.SetGroundCurve(0.3f);

        status = Status.DORMANT;

        dm = GetNode(Game.NATURAL_DISASTERS_PATH) as DisasterManager;

    }

    public override void _Process(float delta)
    {
        //so this is terrible code at the moment. I check if dm has any children and if it does I set the sky to red.
        //I've coded myself into a corner with not being able to check if there is a sandstorm node currently running.
        if (dm.GetChildCount() > 0)
            status = Status.ACTIVE;
        else if (dm.getCurrentDisaster() is DisasterPropertiesSandstorm)
            status = Status.WARNING;
        else status = Status.DORMANT;

    	switch (status) {
    		case Status.DORMANT:
    			timeAlive = 0;
    			break;
    		case Status.WARNING:
    			//Ideally, we'd have the sky fade in andour of red
    			//But we can't fade, so instead we flash
    			timeAlive += delta;
    			if (timeAlive % 1.0 > 0.5) setSkyColour();
    			break;
    		case Status.ACTIVE:
    		    timeAlive = 0;
                setSkyColour();
    			break;
    	}
	}

	private void setSkyColour() {
        Color skyColor = WARNING_COLOUR_SKY;
        Color groundColor = WARNING_COLOUR_GROUND;
        Color skyHorizonColor = skyColor.LinearInterpolate(groundColor, 0.5f);
        Color groundHorizonColor = skyColor.LinearInterpolate(groundColor, 0.5f);

		sky.SetSkyTopColor(skyColor);
        sky.SetSkyHorizonColor(skyHorizonColor);
        sky.SetGroundHorizonColor(groundHorizonColor);
        sky.SetGroundBottomColor(groundColor);
	}


    public static void setStatus(Status newStatus) {
        status = newStatus;
    }
}
