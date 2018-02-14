using Godot;
using System;

public class AnimalBehaviourComponent : BaseComponent
{
    private KinematicBody parent;

    float jumpMagnitude = 5.0f;

    float directionThreshold = 2.0f;
    float timer = 0.0f;

    bool setSpeed = false;

    private void SetRandomDirection()
    {
        Random r = new Random();
        Vector2 d = new Vector2((float)(r.NextDouble() * 2.0 - 1.0), (float)(r.NextDouble() * 2.0 - 1.0));
        GD.Print("Direction set sent: ", d.x, " ", d.y);
        parent.EmitSignal("setDirection", d);
    }

    private void OnTerrainInterference()
    {
        parent.EmitSignal("jump", jumpMagnitude);
    }

    public override void _Ready()
    {
        parent = (KinematicBody)GetParent();
        parent.AddUserSignal("jump");
        parent.AddUserSignal("setDirection");
        parent.AddUserSignal("setSpeed");
    }

    public override void _Process(float delta)
    {
        SetupConnection("terrainInterference", parent, nameof(OnTerrainInterference));

        if (!setSpeed)
        {
            parent.EmitSignal("setSpeed", 150.0f);
            setSpeed = true;
        }

        timer += delta;
        if(timer > directionThreshold)
        {
            timer = 0;
            SetRandomDirection();
        }
    }
}
