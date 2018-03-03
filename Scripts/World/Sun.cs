using Godot;
using System;

public class Sun : DirectionalLight
{
    public override void _Ready()
    {
        ProceduralSky sky = ((GetNode(Game.WORLD_ENVIRO_PATH) as WorldEnvironment).GetEnvironment()).GetSky() as ProceduralSky;

        sky.SunLatitude = -this.RotationDegrees.x;
        sky.SunLongitude = 180 - this.RotationDegrees.y;
    }
}
