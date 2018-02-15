using Godot;
using System;
using System.Collections.Generic;

public class AnimalBehaviourComponent : BaseComponent
{
    public enum Sex
    {
        Male = 0,
        Female = 1,
    }

    public enum Diet
    {
        Herbivore = 0,
        Carnivore = 1,
        Omnivore = 2
    }

    public enum BehaviourState
    {
        Idle = 0,
        Hunting = 1,
    }

    //exports: stats that are generated from spawner
    [Export]
    private int sex; //from Sex enum

    [Export]
    private int diet;

    [Export]
    private int foodChainLevel; //0 is "eaten by everything", no max

    private KinematicBody parent;

    float jumpMagnitude = 5.0f;

    float directionThreshold = 2.0f;
    float timer = 0.0f;

    int frameCount = 0;

    List<PhysicsBody> foodInRange;

    BehaviourState state = BehaviourState.Idle;

    PhysicsBody target;

    private void SetRandomDirection()
    {
        Random r = new Random();
        Vector2 d = new Vector2((float)(r.NextDouble() * 2.0 - 1.0), (float)(r.NextDouble() * 2.0 - 1.0));
        //GD.Print("Direction set sent: ", d.x, " ", d.y);
        parent.EmitSignal("setDirection", d);
    }

    private void OnTerrainInterference()
    {
        parent.EmitSignal("jump", jumpMagnitude);
    }

    public override void _Ready()
    {
        foodInRange = new List<PhysicsBody>();

        parent = (KinematicBody)GetParent();
        parent.AddUserSignal("jump");
        parent.AddUserSignal("setDirection");
        parent.AddUserSignal("setSpeed");
        parent.AddUserSignal("watchFor");
        parent.AddUserSignal("setVisionRange");

        parent.AddToGroup("animals");

        GD.Print("Animal sex: ", ((Sex)sex).ToString());
    }

    protected void SetupInitialisationSignals()
    {
        parent.EmitSignal("setSpeed", 150.0f);

        parent.EmitSignal("watchFor", "plants");
        parent.EmitSignal("watchFor", "animals");
    }

    protected void SetVisionRange(float range)
    {
        parent.EmitSignal("setVisionRange", range);
    }

    protected bool IsFood(PhysicsBody n)
    {
        switch ((Diet)diet)
        {
            case Diet.Carnivore:
                if (n.IsInGroup("animals"))
                {
                    return true;
                }
                break;
            case Diet.Herbivore:
                if (n.IsInGroup("plants"))
                {
                    return true;
                }
                break;
            case Diet.Omnivore:
                return true;
        }
        return false;
    }

    protected void ObjectInRange(PhysicsBody n)
    {
        if (IsFood(n) && (int)n.GetNode("BehaviourComponent").Get("foodChainLevel") < foodChainLevel)
        {
            foodInRange.Add(n);
        }
    }

    protected void ObjectOutOfRange(PhysicsBody n)
    {
        if (IsFood(n))
        {
            //List is implemented as an array, so this could get quite slow
            //Could change to hash map if this is a bottleneck
            foodInRange.Remove(n);
        }
    }

    public override void _Process(float delta)
    {
        if(frameCount == 1)
        {
            // initialise things that rely on signals only if not the first frame
            // this guarantees that other components will have set up their connections
            SetupInitialisationSignals();

            frameCount++;
        }
        else if(frameCount == 0)
        {
            frameCount++;
        }

        SetupConnection("terrainInterference", parent, nameof(OnTerrainInterference));
        SetupConnection("objectInRange", parent, nameof(ObjectInRange));
        SetupConnection("objectOutOfRange", parent, nameof(ObjectOutOfRange));

        if (state == BehaviourState.Idle)
        {
            timer += delta;
            if (timer > directionThreshold)
            {
                timer = 0;
                SetRandomDirection();
            }
        }
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);

        if(state == BehaviourState.Idle)
        {
            foreach (PhysicsBody b in foodInRange)
            {
                GD.Print("IDLE: Food in range, can I see it?");
                GD.Print("IDLE: Target position: ", b.GetTranslation(), "My position: ", parent.GetTranslation());
                //Identify whether we can see the target by raycasting
                PhysicsDirectSpaceState spaceState = b.GetWorld().GetDirectSpaceState();
                var result = spaceState.IntersectRay(parent.GetTranslation(), b.GetTranslation(), new[] { parent, b });

                if(result.Count == 0)
                {
                    GD.Print("Yes!");
                    target = b;
                    state = BehaviourState.Hunting;
                }
                else
                {
                    GD.Print("Nope. Ray hit: ",((Node)result["collider"]).GetName());
                }
            }
        }
        else if (state == BehaviourState.Hunting)
        {
            // Check for line of sight and whether object is still in range.
            PhysicsDirectSpaceState spaceState = target.GetWorld().GetDirectSpaceState();
            var result = spaceState.IntersectRay(target.GetTranslation(), parent.GetTranslation(), new[] { parent, target });

            if (result.Count != 0 || target.ToGlobal(target.GetTranslation()).DistanceSquaredTo(parent.ToGlobal(parent.GetTranslation())) > 60 * 60)
            {
                GD.Print("Hunting, but I can't see him any more :(");
                state = BehaviourState.Idle;
            }
            else
            {
                GD.Print("Hunting. Target: ", target.GetName(), "Me: ", parent.GetParent().GetName());
                GD.Print("Hunting. Target position: ", target.GetTranslation(), "My position: ",parent.GetTranslation());
                Vector3 direction = target.GetTranslation() - parent.GetTranslation();
                GD.Print("So the direction: ", new Vector2(direction.x, direction.z));
                parent.EmitSignal("setDirection", new Vector2(direction.x, direction.z));
            }
        }
    }
}
