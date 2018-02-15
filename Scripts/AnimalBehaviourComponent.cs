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
        Breeding = 2,
    }

    //exports: stats that are generated from spawner
    [Export]
    private int sex; //from Sex enum

    [Export]
    private int diet; //from Diet enum

    [Export]
    private int foodChainLevel; //0 is "eaten by everything", no max

    private KinematicBody parent;

    private float jumpMagnitude = 5.0f;

    private const float directionThreshold = 2.0f;
    private float directionTimer = 0.0f;

    private const float breedingThreshold = 5.0f;
    private float breedingTimer = 0.0f;

    private const float breedingSignalThreshold = 3.0f;
    private float breedingSignalTimer = 0.0f;
    private bool breedingSignalled = false;

    private const float breedingCollisionsThreshold = 2.0f;
    private float breedingCollisionTimer = 0.0f;
    private bool breedingCollision = false;

    private const float satiatedBreedThreshold = 50.0f;

    private int frameCount = 0;

    private List<PhysicsBody> foodInRange;
    private List<PhysicsBody> breedableTargets;

    private BehaviourState state = BehaviourState.Idle;

    private PhysicsBody target;
    private PhysicsBody breedingTarget;

    [Export]
    private string breedingTargetName = "";

    [Export]
    float satiated = 100.0f; //100 is max, 0 is starved to death

    [Export]
    int breedability;

    [Export]
    string presetName;

    const float timeToDeath = 50.0f;

    // TODO NEXT: Breeding. Random chance animal (defined in preset) will feel like breeding with 5sec cooldown if satiated enough, if in sight of opposite sex.
    // If test passes, heads towards target, and when within range, send request. Target can accept or deny request.
    // If request accepted, continue to head towards each other, and when collided, wait 3 seconds, then if both still alive, produce baby.
    // 50% chance for gender of baby, need to add preset names.
    // Then make sure works on laptop. 

    // Left to do: Add different models. Add different basic behaviours to models (eg movement). Improve AI a bit. This can be done tomorrow (got 11-16).
    // No plants yet. That's fine, just give herbivores a long time-to-death for demo purposes.

    // Also, add rotations.

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
        breedableTargets = new List<PhysicsBody>();

        parent = (KinematicBody)GetParent();
        parent.AddUserSignal("jump");
        parent.AddUserSignal("setDirection");
        parent.AddUserSignal("setSpeed");
        parent.AddUserSignal("watchFor");
        parent.AddUserSignal("setVisionRange");
        parent.AddUserSignal("attemptBreeding");

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
        Node behaviourComponent = n.GetNode("BehaviourComponent");
        if (IsFood(n) && (int)behaviourComponent.Get("foodChainLevel") < foodChainLevel)
        {
            foodInRange.Add(n);
        }else if (behaviourComponent.Get("presetName").Equals(presetName) && (int)behaviourComponent.Get("sex") != sex)
        {
            breedableTargets.Add(n);
        }
    }

    protected void ObjectOutOfRange(PhysicsBody n)
    {
        Node behaviourComponent = n.GetNode("BehaviourComponent");
        if (IsFood(n) && (int)behaviourComponent.Get("foodChainLevel") < foodChainLevel)
        {
            //List is implemented as an array, so could get expensive. Could change to HashMap.
            foodInRange.Remove(n);
        }
        else if (behaviourComponent.Get("presetName").Equals(presetName) && (int)behaviourComponent.Get("sex") != sex)
        {
            breedableTargets.Remove(n);
        }
    }

    protected void Collided(KinematicCollision collision)
    {
        if(state == BehaviourState.Hunting && target != null && collision.Collider.Equals(target))
        {
            eat(collision.Collider);            
        }else if(state == BehaviourState.Breeding && breedingTarget != null && collision.Collider.Equals(breedingTarget)) //is this right?
        {
            breedingCollision = true;
            if(breedingCollisionTimer >= breedingCollisionsThreshold)
            {
                GD.Print("Breed! If female.");
                if(sex == (int)Sex.Female)
                {
                    Node spawnNode = GetTree().GetRoot().GetNode("Game").GetNode("AnimalSpawner");
                    Random r = new Random();
                    int nextSex = r.Next(0, 1);

                    spawnNode.Call("SpawnAnimal",presetName,(AnimalBehaviourComponent.Sex)nextSex, parent.GetTranslation() + new Vector3(0.0f,20.0f,0.0f));
                }

                breedingTargetName = "";
                state = BehaviourState.Idle;
            }
        }
    }

    protected void eat(Godot.Object nom)
    {
        GD.Print("Nom!");
        satiated = Math.Max(100.0f, satiated + 20.0f);
        if(nom is Node)
        {
            ((Node)nom).QueueFree();          
        }
        state = BehaviourState.Idle;
    }

    protected void hungry(float delta)
    {
        satiated -= delta * (100.0f/timeToDeath);

        if(satiated <= 0.0f)
        {
            GD.Print("Starved to death!");
            parent.QueueFree();
        }
    }

    protected void AttemptBreedState()
    {
        //only attempt breed if there is a breedable target in sight
        PhysicsBody found = null;

        foreach(PhysicsBody body in breedableTargets)
        {
            if (body == null) continue;
            // Identify whether we can see the target by raycasting
            PhysicsDirectSpaceState spaceState = body.GetWorld().GetDirectSpaceState();
            var result = spaceState.IntersectRay(parent.GetTranslation(), body.GetTranslation(), new[] { parent, body });

            if (result.Count == 0)
            {
                found = body;
                break;
            }
        }

        if (found == null) return;

        breedingTimer = 0;

        Random r = new Random();
        int n = r.Next(0, 100);
        if((sex == (int)Sex.Female)){
            n = 100;
        }
        if(n < breedability)
        {
            breedingTarget = found;
            breedingTargetName = breedingTarget.GetName();

            GD.Print("Found partner to attempt to breed with! Path: ", breedingTargetName);

            SetupBreedingState();
        }
        else
        {
            GD.Print(parent.GetName(), ": Found partner, but decided not to pursue.");
        }
    }

    protected void SetupBreedingState()
    {
        breedingSignalTimer = 0.0f;
        breedingSignalled = false;
        breedingCollisionTimer = 0.0f;
        breedingCollision = false;

        state = BehaviourState.Breeding;
    }

    protected void AttemptBreeding(KinematicBody body)
    {
        GD.Print(parent.GetName(), ": Breeding request received!");
        if (satiated < satiatedBreedThreshold) return;
        Random r = new Random();
        int n = r.Next(0, 100);
        if(n < breedability)
        {
            GD.Print(parent.GetName(),": Breeding request approved!");
            breedingTarget = body;
            breedingTargetName = body.GetName();

            SetupBreedingState();
            breedingSignalled = true;
        }
        else
        {
            GD.Print(parent.GetName(),": Breeding request rejected");
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
        SetupConnection("collided", parent, nameof(Collided));
        SetupConnection("attemptBreeding", parent, nameof(AttemptBreeding));

        //cleanse dead objects
        foodInRange.RemoveAll(p => p == null);
        breedableTargets.RemoveAll(p => p == null);

        if (state == BehaviourState.Idle)
        {
            directionTimer += delta;
            breedingTimer += delta;
            if (directionTimer > directionThreshold)
            {
                directionTimer = 0;
                SetRandomDirection();
            }
            if(breedingTimer > breedingThreshold && satiated > satiatedBreedThreshold)
            {
                AttemptBreedState();
            }
        }
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);

        if(state == BehaviourState.Idle)
        {
            foreach (PhysicsBody b in foodInRange) //Could randomise this?
            {
                if (b == null) continue;
                // Identify whether we can see the target by raycasting
                PhysicsDirectSpaceState spaceState = b.GetWorld().GetDirectSpaceState();
                var result = spaceState.IntersectRay(parent.GetTranslation(), b.GetTranslation(), new[] { parent, b });

                if(result.Count == 0)
                {
                    target = b;
                    state = BehaviourState.Hunting;
                    break;
                }
            }
        }
        else if (state == BehaviourState.Hunting)
        {
            if(target == null)
            {
                state = BehaviourState.Idle;
            }
            else
            {
                // Check for line of sight and whether object is still in range.
                PhysicsDirectSpaceState spaceState = target.GetWorld().GetDirectSpaceState();
                var result = spaceState.IntersectRay(target.GetTranslation(), parent.GetTranslation(), new[] { parent, target });

                if (result.Count != 0 || target.GetTranslation().DistanceSquaredTo(parent.GetTranslation()) > 60 * 60)
                {
                    state = BehaviourState.Idle;
                }
                else
                {
                    Vector3 direction = target.GetTranslation() - parent.GetTranslation();
                    parent.EmitSignal("setDirection", new Vector2(direction.x, direction.z));
                }
            }        
        }else if(state == BehaviourState.Breeding)
        {
            if(breedingTarget == null)
            {
                state = BehaviourState.Idle;
            }
            else
            {
                float distanceSquared = breedingTarget.GetTranslation().DistanceSquaredTo(parent.GetTranslation());
                if (distanceSquared <= 30 * 30)
                {
                    // Close enough: try match!
                    if (!breedingSignalled)
                    {
                        GD.Print(parent.GetName(), ": Close enough to try match! Signalling...");
                        breedingTarget.EmitSignal("attemptBreeding", parent);
                        breedingSignalled = true;
                    }

                    if (breedingCollision)
                    {
                        breedingCollisionTimer += delta;
                        if(breedingCollisionTimer >= breedingCollisionsThreshold + 1.0f)
                        {
                            //partner must have been eaten...
                            GD.Print(parent.GetName(), ": Where did you go ? :(");
                            state = BehaviourState.Idle;
                        }
                    }

                    string targetString = (string)breedingTarget.GetNode("BehaviourComponent").Get("breedingTargetName");

                    if (targetString.Equals(parent.GetName()))
                    {
                        Vector3 direction = breedingTarget.GetTranslation() - parent.GetTranslation();
                        parent.EmitSignal("setDirection", new Vector2(direction.x, direction.z));
                    }
                    else
                    {
                        parent.EmitSignal("setDirection", new Vector2(0, 0));
                        breedingSignalTimer += delta;
                        if (breedingSignalTimer > breedingSignalThreshold)
                        {
                            // Must have been rejected!
                            state = BehaviourState.Idle;

                            GD.Print(parent.GetName(), ": Detected rejection. Idling.");
                        }
                    }
                }
            }
        }
    }
}