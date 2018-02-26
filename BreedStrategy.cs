using Godot;
using System;
using System.Collections.Generic;

public class BreedStrategy : BaseStrategy
{
    private List<PhysicsBody> breedableTargets;

    private PhysicsBody target;
    private string targetName {
        get { return targetName; }
        set { targetName = value; component.Set("BreedingTargetName", value); }
    }

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
        SeenTarget,
        ApproachedTarget,
        WaitingForResponse,
        GoingForBreed,
        Breeding
    }

    private BreedState state;

    private void SetState(BreedState state)
    {
        switch (state)
        {
            case BreedState.SeenTarget:
                break;
            case BreedState.ApproachedTarget:               
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

        component.Set("breedingTargetName", "");
        component.body.AddUserSignal("attemptingBreeding");
    }

    public override void ObjectInRange(PhysicsBody body)
    {
        base.ObjectInRange(body);
        Node behaviourComponent = body.GetNode("BehaviourComponent");
        if (behaviourComponent.Get("presetName").Equals(component.presetName) && (int)behaviourComponent.Get("sex") != component.sex)
        {
            breedableTargets.Add(body);
        }
    }

    public override void ObjectOutOfRange(PhysicsBody body)
    {
        base.ObjectOutOfRange(body);
        Node behaviourComponent = body.GetNode("BehaviourComponent");
        if(behaviourComponent.Get("presetName").Equals(component.presetName) && (int)behaviourComponent.Get("sex") != component.sex)
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
                        Node spawnNode = GetTree().GetRoot().GetNode("Game").GetNode("AnimalSpawner");
                        Random r = new Random();
                        int nextSex = r.Next(0, 2);

                        spawnNode.Call("SpawnAnimal", component.presetName, (AnimalBehaviourComponent.Sex)nextSex, component.body.GetTranslation() + new Vector3(0.0f, 20.0f, 0.0f));
                        component.satiated -= 20.0f;
                    }
                }
            }
        }
    }

    public override List<Tuple<string, Node, string>> GetConnections()
    {
        var l = new List<Tuple<string, Node, string>>();
        l.Add(Tuple.Create("attemptBreeding", component.GetParent(), nameof(AttemptBreeding)));

        return l;
    }

    public override void StartState(params object[] args)
    {
        SetState(BreedState.SeenTarget);
        target = (PhysicsBody)args[0];
        targetName = target.GetName();
    } 

    protected void AttemptBreeding(KinematicBody otherBody)
    {
        if (otherBody.GetName() == targetName)
        {
            SetState(BreedState.GoingForBreed);
        }
        else
        {
            if (!active || state == BreedState.SeenTarget)
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

    public override void Process(float delta) //TODO: need BehaviourComponent to setup connections on strategy's behalf.
    {
        breedableTargets.RemoveAll(p => p == null);
    }

    public override void PhysicsProcess(float delta)
    {
        if(target == null)
        {
            active = false;
            return;
        }
        if (state == BreedState.ApproachedTarget)
        {
            float distanceSquared = target.GetTranslation().DistanceSquaredTo(component.body.GetTranslation());
            if (distanceSquared <= 30 * 30)
            {
                target.EmitSignal("attemptingBreeding", component.body);
                state = BreedState.WaitingForResponse;
            }
        }
        else if (state == BreedState.WaitingForResponse)
        {
            string targetString = (string)target.GetNode("BehaviourComponent").Get("breedingTargetName");

            if (targetString.Equals(component.body.GetName()))
            {
                Vector3 direction = target.GetTranslation() - component.body.GetTranslation();
                component.body.EmitSignal("setDirection", new Vector2(direction.x, direction.z));
            }
            else
            {
                component.body.EmitSignal("setDirection", new Vector2(0, 0));
                waitingTimer += delta;
                if (waitingTimer > waitingThreshold)
                {
                    // Must have been rejected!
                    active = false;

                    GD.Print(component.body.GetName(), ": Detected rejection. Idling.");
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
