using Godot;
using System;
using System.Collections.Generic;

public class PhysicsComponent : BaseComponent
{
    private const float gravity = 20.0f;
    private const float terminal = 200.0f;

    private KinematicBody body;
    private Vector3 velocity;

    private float speed = 0.0f;
    private Vector2 direction;

    private const float terrainInterferenceEpsilon = 0.5f;

    private bool tojump = false;
    private float jumpMagnitude;

    private Area area;

    private List<string> watchList;

    private int frameNum = 0;

    public PhysicsComponent(Entity parent) : base(parent) { }

    private void Jump(object[] args)
    {
        float magnitude = (float)args[0];
        if (body.IsOnFloor())
        {
            tojump = true;
            jumpMagnitude = magnitude;
        }
    }

    private void SetDirection(object[] args)
    {
        Vector2 d = (Vector2)args[0];
        
        direction = d.Normalized();
        if (d.x != 0.0f || d.y != 0.0f)
        {
            body.LookAt(body.GetTranslation() + new Vector3(direction.x, 0, direction.y) * 100.0f, new Vector3(0, 1, 0));
        }
    }

    private void SetSpeed(object[] args)
    {
        speed = (float)args[0];
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
                parent.SendMessage("objectInRange", pobj);
            }
        }    
    }

    private void ObjectOutOfRange(Godot.Object obj)
    {
        if (obj is PhysicsBody)
        {
            PhysicsBody pobj = (PhysicsBody)obj;
            if (IsWatching(pobj)) { 
                parent.SendMessage("objectOutOfRange", pobj);
                return;
            }
        }
    }

    private void WatchFor(object[] args)
    {
        watchList.Add((string)args[0]);
    }

    public override void Ready()
    {
        watchList = new List<String>();

        body = (KinematicBody)parent.GetParent();

        parent.RegisterListener("jump", Jump);
        parent.RegisterListener("setDirection", SetDirection);
        parent.RegisterListener("setSpeed", SetSpeed);
        parent.RegisterListener("watchFor", WatchFor);

        //for detection objects in a range

        //TODO: Generate this area in code. Could not figure out how to generate areas in code or get the shapes from existing areas,
        //but it is possible. As a result, currently the area must be set in the animal scene file, and the range cannot be changed through code.
        area = (Area)body.GetNode("Area");

        area.Connect("body_entered", this, nameof(ObjectInRange));
        area.Connect("body_exited", this, nameof(ObjectOutOfRange));        
    }

    public override void Process(float delta)
    {

        //This sort of initialisation needs a refactor, but suffices for now.
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
                    //parent.SendMessage("objectInRange", body);
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

        Vector3 newVelocity = body.MoveAndSlide(velocity, new Vector3(0.0f, 1.0f, 0.0f));

        float xDif = Math.Abs(velocity.x - newVelocity.x);
        float zDif = Math.Abs(velocity.z - newVelocity.z);

        if(xDif > terrainInterferenceEpsilon || zDif > terrainInterferenceEpsilon)
        {
            parent.SendMessage("terrainInterference");
        }

        int numCollisions = body.GetSlideCount();
        for(int i = 0; i < numCollisions; i++)
        {
            parent.SendMessage("collided", body.GetSlideCollision(i));
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

    public override void PhysicsProcess(float delta)
    {

    }
}
