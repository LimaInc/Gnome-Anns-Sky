using Godot;
using System;
using System.Collections.Generic;

public class PhysicsComponent : BaseComponent
{
    private const float gravity = 20.0f;
    private const float terminal = 200.0f;

    private KinematicBody parent;
    private Vector3 velocity;

    private float speed = 0.0f;
    private Vector2 direction;

    private const float terrainInterferenceEpsilon = 0.5f;

    private bool tojump = false;
    private float jumpMagnitude;

    private Area area;

    private List<string> watchList;

    private int frameNum = 0;

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
        //GD.Print("Direction set received: ", d.x, " ", d.y);
        direction = d.Normalized();
        //GD.Print("Direction set: ", direction);
    }

    private void SetSpeed(float s)
    {
        speed = s;
        GD.Print("Speed set: ", speed);
    }

    private void SetVisionRange(float r)
    {
        GD.Printerr("SetVisionRange is not implemented yet.");
        //visionRange = r;
        //areaShape.SetExtents(new Vector3(visionRange, visionRange, visionRange));
    }

    private bool IsWatching(PhysicsBody body)
    {
        foreach (string gName in watchList)
        {
            if (body.IsInGroup(gName) && body.GetInstanceId() != parent.GetInstanceId())
            {
                return true;
            }
        }
        return false;
    }

    private void ObjectInRange(Godot.Object obj)
    {
        if (obj is PhysicsBody)
        {
            PhysicsBody pobj = (PhysicsBody)obj;
            if (IsWatching(pobj)){
                parent.EmitSignal("objectInRange", pobj);
            }
        }    
    }

    private void ObjectOutOfRange(Godot.Object obj)
    {
        if (obj is PhysicsBody)
        {
            PhysicsBody pobj = (PhysicsBody)obj;
            if (IsWatching(pobj)) { 
                parent.EmitSignal("objectOutOfRange", pobj);
                return;
            }
        }
    }

    private void WatchFor(string groupName)
    {
        watchList.Add(groupName);
    }

    //TODO: add "watchfor" functionality, so once signal received to watch for a certain group, throws signals when those groups get in range
    //then implement the raycast in the behaviour component
    //then make eat, mate, etc

    public override void _Ready()
    {
        watchList = new List<String>();

        GD.Print("Physics component readying... ");
        parent = (KinematicBody)GetParent(); //downcast!

        //set up the signals we want to broadcast
        parent.AddUserSignal("terrainInterference");
        parent.AddUserSignal("collided");
        parent.AddUserSignal("objectInRange");
        parent.AddUserSignal("objectOutOfRange");

        //for detection objects in a range

        //TODO: Generate this area in code. Could not figure out how to generate areas in code or get the shapes from existing areas,
        //but it is possible. As a result, currently the area must be set in the animal scene file, and the range cannot be changed through code.
        area = (Area)parent.GetNode("Area");

        area.Connect("body_entered", this, nameof(ObjectInRange));
        area.Connect("body_exited", this, nameof(ObjectOutOfRange));        
    }

    public override void _Process(float delta)
    {
        // initialise connections that we use
        SetupConnection("jump", parent, nameof(Jump));
        SetupConnection("setDirection", parent, nameof(SetDirection));
        SetupConnection("setSpeed", parent, nameof(SetSpeed));
        SetupConnection("setVisionRange", parent, nameof(SetVisionRange));
        SetupConnection("watchFor", parent, nameof(WatchFor));

        if(frameNum == 2)
        {
            // Initialise things that need to wait for emitting signals here.
            // To give other components a chance to set up and fire.

            // get all objects that start in range which could not fire the signals
            Array bodies = area.GetOverlappingBodies();

            foreach(PhysicsBody body in bodies)
            {
                if (IsWatching(body))
                {
                    parent.EmitSignal("objectInRange", body);
                }
            }


            frameNum++;
        }
        else if(frameNum == 1)
        {
            // Initialise things that rely on emitting custom signals here.
            frameNum++;
        }
        else if(frameNum == 0)
        {
            frameNum++;
        }

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
