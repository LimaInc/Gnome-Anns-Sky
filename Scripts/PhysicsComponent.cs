using Godot;
using System;

public class PhysicsComponent : BaseComponent
{
    private float gravity = 20.0f;
    private float terminal = 200.0f;

    private KinematicBody parent;
    private Vector3 velocity;

    private float speed = 0.0f;
    private Vector2 direction;

    private float terrainInterferenceEpsilon = 0.5f;

    private bool tojump = false;
    private float jumpMagnitude;

    private void Jump(float magnitude)
    {
        if (parent.IsOnFloor())
        {
            tojump = true;
            jumpMagnitude = magnitude;
        }
    }

    private void SetDirection(Vector2 d)
    {
        GD.Print("Direction set received: ", d.x, " ", d.y);
        direction = d.Normalized();
        GD.Print("Direction set: ", direction);
    }

    private void SetSpeed(float s)
    {
        speed = s;
        GD.Print("Speed set: ", speed);
    }

    public override void _Ready()
    {
        GD.Print("Physics component readying... ");
        parent = (KinematicBody)GetParent(); //downcast!

        //set up the signals we want to broadcast
        parent.AddUserSignal("terrainInterference");
        parent.AddUserSignal("collided"); 
    }

    public override void _Process(float delta)
    {
        // initialise connections that we use
        SetupConnection("jump", parent, nameof(Jump));
        SetupConnection("setDirection", parent, nameof(SetDirection));
        SetupConnection("setSpeed", parent, nameof(SetSpeed));

        velocity.x = direction.x * speed * delta;
        velocity.z = direction.y * speed * delta;
        velocity.y -= gravity * delta;

        if(velocity.y < -terminal)
        {
            velocity.y = -terminal;
        }

     //   GD.Print("Attempted velocity: ", velocity.x, " ", velocity.y, " ", velocity.z);
        Vector3 newVelocity = parent.MoveAndSlide(velocity, new Vector3(0.0f, 1.0f, 0.0f));
     //   GD.Print("New velocity: ", newVelocity.x, " ", newVelocity.y, " ", newVelocity.z);

        float xDif = Math.Abs(velocity.x - newVelocity.x);
        float zDif = Math.Abs(velocity.z - newVelocity.z);

        if(xDif > terrainInterferenceEpsilon || zDif > terrainInterferenceEpsilon)
        {
            parent.EmitSignal("terrainInterference");
        }

        int numCollisions = parent.GetSlideCount();
        for(int i = 0; i < numCollisions; i++)
        {
            parent.EmitSignal("collided", parent.GetSlideCollision(i));
        }

        if (tojump)
        {
            velocity.y = jumpMagnitude;
            tojump = false;
        }
        else
        {
            velocity.y = newVelocity.y;
        }
    }
}
