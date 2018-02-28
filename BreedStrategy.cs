using Godot;
using System;
using System.Collections.Generic;

public class BreedStrategy : BaseStrategy
{
    private List<PhysicsBody> breedableTargets;

    private PhysicsBody target;

    private string targetName = "";
    private string GetBreedingTargetName() { return targetName; }

    private float breedingTimer = 0.0f;
    private const float breedingThreshold = 2.0f;

    private float waitingTimer = 0.0f;
    private const float waitingThreshold = 3.0f;

    private const float satiatedThreshold = 50.0f;

    public BreedStrategy(AnimalBehaviourComponent component) : base(component)
    {

    }

    enum BreedState
    {
        ApproachingTarget,
        WaitingForResponse,
        GoingForBreed,
        Breeding
    }

    private BreedState state;

    private void SetState(BreedState state)
    {
        switch (state)
        {
            case BreedState.ApproachingTarget:               
                break;
            case BreedState.GoingForBreed:
                break;
            case BreedState.WaitingForResponse:
                waitingTimer = 0.0f;
                break;
            case BreedState.Breeding:
                breedingTimer = 0.0f;
                break;
        }
        this.state = state;
    }

    public override void Ready()
    {
        breedableTargets = new List<PhysicsBody>();
        component.body.AddUserSignal("attemptingBreeding");

        component.RegisterCustomProperty("breedingTargetName", this, nameof(GetBreedingTargetName));
        component.RegisterCustomProperty("breedingRequest", this, nameof(AttemptBreeding));

    }

    public override void ObjectInRange(PhysicsBody body)
    {
        base.ObjectInRange(body);
        Node behaviourComponent = body.GetNode("BehaviourComponent");
        if (behaviourComponent.Get("presetName").Equals(component.presetName) && (int)behaviourComponent.Get("_sex") != component.sex)
        {
            breedableTargets.Add(body);
        }
    }

    public override void ObjectOutOfRange(PhysicsBody body)
    {
        base.ObjectOutOfRange(body);
        Node behaviourComponent = body.GetNode("BehaviourComponent");
        if(behaviourComponent.Get("presetName").Equals(component.presetName) && (int)behaviourComponent.Get("_sex") != component.sex)
        {
            breedableTargets.Remove(body);
        }
    }

    public override void Collided(KinematicCollision collision)
    {
        base.Collided(collision);

        if(target != null && collision.Collider.Equals(target))
        {
            if(state == BreedState.GoingForBreed)
            {
                state = BreedState.Breeding;
            }
            else if(state == BreedState.Breeding)
            {
                if(breedingTimer > breedingThreshold)
                {
                    if (component.sex == (int)AnimalBehaviourComponent.Sex.Female)
                    {
                        Node spawnNode = component.GetTree().GetRoot().GetNode("Game").GetNode("AnimalSpawner");
                        Random r = new Random();
                        int nextSex = r.Next(0, 2);

                        spawnNode.Call("SpawnAnimal", component.presetName, (AnimalBehaviourComponent.Sex)nextSex, component.body.GetTranslation() + new Vector3(0.0f, 20.0f, 0.0f));
                        component.satiated -= 20.0f;
                    }
                }
            }
        }
    }

    public override void StartState(params object[] args)
    {
        base.StartState(args);
        SetState(BreedState.ApproachingTarget);
        target = (PhysicsBody)args[0];
        targetName = target.GetName();
    } 

    protected void AttemptBreeding(object[] args)
    {
        KinematicBody otherBody = (KinematicBody)args[0];
        GD.Print(component.body.GetName(), ": Received breed request from ", otherBody.GetName());
        if (otherBody.GetName() == targetName)
        {
            GD.Print("Already had that target!");
            SetState(BreedState.GoingForBreed);
        }
        else
        {
            if (!active || state == BreedState.ApproachingTarget)
            {
                if (component.satiated < satiatedThreshold) return;
                Random r = new Random();
                int n = r.Next(0, 100);
                if (n < component.breedability)
                {
                    GD.Print(component.GetParent().GetName(), ": Breeding request approved!");
                    target = otherBody;
                    targetName = otherBody.GetName();
                    active = true;

                    SetState(BreedState.GoingForBreed);
                }
                else
                {
                    GD.Print(component.body.GetName(), ": Breeding request rejected");
                }
            }
        }
    }

    public override void Process(float delta)
    {
        breedableTargets.RemoveAll(p => p == null);
    }

    public PhysicsBody ShouldBreedState()
    {
        //only attempt breed if there is a breedable target in sight
        PhysicsBody found = null;

        foreach (PhysicsBody body in breedableTargets)
        {
            if (body == null) continue;
            // Identify whether we can see the target by raycasting
            PhysicsDirectSpaceState spaceState = body.GetWorld().GetDirectSpaceState();
            var result = spaceState.IntersectRay(component.body.GetTranslation(), body.GetTranslation(), new[] { component.body, body });

            if (result.Count == 0)
            {
                found = body;
                break;
            }
        }

        if (found == null) return null;

        breedingTimer = 0;

        Random r = new Random();
        int n = r.Next(0, 100);
        if ((component.sex == (int)AnimalBehaviourComponent.Sex.Female))
        {
            n = 100;
        }
        if (n < component.breedability)
        {
            GD.Print("Found partner to attempt to breed with! Path: ", found.GetName());
            return found;
        }

        return null;
    }

    public override void PhysicsProcess(float delta)
    {
        if(target == null)
        {
            active = false;
            return;
        }
        if (state == BreedState.ApproachingTarget)
        {
            Vector3 direction = target.GetTranslation() - component.body.GetTranslation();
            component.body.EmitSignal("setDirection", new Vector2(direction.x, direction.z));

            float distanceSquared = direction.LengthSquared();
            if (distanceSquared <= 30 * 30)
            {
                GD.Print(component.body.GetName(), ": Close enough to send request. Sending!");
                Node c = target.GetNode("BehaviourComponent");
                GD.Print("Node:", c);
                //target.GetNode("BehaviourComponent").Call("GetCustomProperty", "breedingRequest", new[] { component.body });
                SetState(BreedState.WaitingForResponse);
            }
        }
        else if (state == BreedState.WaitingForResponse)
        {

            AnimalBehaviourComponent behaviour = (AnimalBehaviourComponent)target.GetNode("BehaviourComponent");
            string targetString = (string)target.GetNode("BehaviourComponent")
                
                
                .Call("GetCustomProperty", "breedingTargetName", new string[] { "lol", "hello" });

            targetString = "test";
            if (targetString.Equals(component.body.GetName()))
            {
                GD.Print(component.body.GetName(), ": Detected acceptance!");
                Vector3 direction = target.GetTranslation() - component.body.GetTranslation();
                component.body.EmitSignal("setDirection", new Vector2(direction.x, direction.z));
                SetState(BreedState.Breeding);
            }
            else
            {
                component.body.EmitSignal("setDirection", new Vector2(0, 0));
                waitingTimer += delta;
                if (waitingTimer > waitingThreshold)
                {
                    GD.Print(component.body.GetName(), ": Detected rejection. Idling.");    
                    // Must have been rejected!
                    active = false;

                    
                }
            }
        }
        else if (state == BreedState.Breeding)
        {
            breedingTimer += delta;

            if (breedingTimer > breedingThreshold + 1.0f)
            {
                // partner must have been eaten...
                GD.Print(component.body.GetName(), ": Where did you go? :(");
                active = false;
            }
        }
    }
}
