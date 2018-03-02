using Godot;
using System;

public class Sun : DirectionalLight
{
    WorldEnvironment worldEnvironment;
    Godot.Environment environment;
    ProceduralSky sky;

    public override void _Ready()
    {
        worldEnvironment = GetNode(Game.WORLD_ENVIRO_PATH) as WorldEnvironment;
        environment = worldEnvironment.GetEnvironment();

        sky = environment.GetSky() as ProceduralSky;

        sky.SunLatitude = -this.RotationDegrees.x;
        sky.SunLongitude = 180 - this.RotationDegrees.y;
    }
}
