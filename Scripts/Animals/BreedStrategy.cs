using Godot;
using System;
using System.Collections.Generic;

public class BreedStrategy : BaseStrategy
{
    private List<PhysicsBody> breedableTargets;

    private PhysicsBody target;

    private string targetName = "";

    private float breedingTimer = 0.0f;
    private const float breedingThreshold = 2.0f;

    private float waitingTimer = 0.0f;
    private const float waitingThreshold = 3.0f;

    private const float satiatedThreshold = 80.0f;

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
        GD.Print(component.body.GetName(), ": Set state: ", state.ToString());
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

        component.parent.RegisterListener("acceptBreed", BreedAccepted);
        component.parent.RegisterListener("breedingRequest", AttemptBreeding);
        component.parent.RegisterListener("objectInRange", ObjectInRange);
        component.parent.RegisterListener("objectOutOfRange", ObjectOutOfRange);
        component.parent.RegisterListener("collided", Collided);
    }

    public void ObjectInRange(object[] args)
    {
        PhysicsBody body = (PhysicsBody)args[0];
        AnimalBehaviourComponent behaviourComponent = ((Entity)body.GetNode("Entity")).GetComponent<AnimalBehaviourComponent>();
        if (behaviourComponent.presetName.Equals(component.presetName) && behaviourComponent.sex != component.sex)
        {
            breedableTargets.Add(body);
        }
    }

    public void ObjectOutOfRange(object[] args)
    {
        PhysicsBody body = (PhysicsBody)args[0];
        AnimalBehaviourComponent behaviourComponent = ((Entity)body.GetNode("Entity")).GetComponent<AnimalBehaviourComponent>();
        if(behaviourComponent.presetName.Equals(component.presetName) && behaviourComponent.sex != component.sex)
        {
            breedableTargets.Remove(body);
        }
    }

    public void Collided(object[] args)
    {
        if (active)
        {
            KinematicCollision collision = (KinematicCollision)args[0];

            if (target != null && collision.Collider.Equals(target))
            {
                if (state == BreedState.GoingForBreed)
                {
                    SetState(BreedState.Breeding);
                }
                else if (state == BreedState.Breeding)
                {
                    if (breedingTimer > breedingThreshold)
                    {
                        if (component.sex == AnimalBehaviourComponent.Sex.Female)
                        {
                            Node spawnNode = component.parent.GetTree().GetRoot().GetNode("Game").GetNode("AnimalSpawner");
                            Random r = new Random();
                            int nextSex = r.Next(0, 2);
                            spawnNode.Call("SpawnAnimal", component.presetName, (AnimalBehaviourComponent.Sex)nextSex, component.body.GetTranslation() + new Vector3(0.0f, 20.0f, 0.0f));
                            component.satiated -= 20.0f;
                        }
                        active = false;
                    }
                }
            }
        }
    }

    public override void StartState(params object[] args)
    {
        base.StartState(args);
        GD.Print(component.body.GetName(), ": Starting state");
        SetState(BreedState.ApproachingTarget);
        target = (PhysicsBody)args[0];
        targetName = target.GetName();
    } 

    private void AttemptBreeding(object[] args)
    {
        KinematicBody otherBody = (KinematicBody)args[0];
        GD.Print(component.body.GetName(), ": Received breed request from ", otherBody.GetName());
        if (otherBody.GetName() == targetName)
        {
            GD.Print("Already had that target!");
            ((Entity)otherBody.GetNode("Entity")).SendMessage("acceptBreed");
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
                    GD.Print(component.body.GetName(), ": Breeding request approved!");
                    ((Entity)otherBody.GetNode("Entity")).SendMessage("acceptBreed");
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

    private void BreedAccepted(object[] args)
    {
        GD.Print(component.body.GetName(),": Breed accepted called");
        SetState(BreedState.GoingForBreed);
        GD.Print(component.body.GetName(), ": State: ", state.ToString());
    } 

    public override void Process(float delta)
    {
        breedableTargets.RemoveAll(p => p == null);
    }

    public PhysicsBody ShouldBreedState()
    {
        if(component.satiated > satiatedThreshold)
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

            GD.Print(component.body.GetName(), "Found potential breed target");

            Random r = new Random();
            int n = r.Next(0, 100);

            if (n < component.breedability)
            {
                GD.Print("Found partner to attempt to breed with! Path: ", found.GetName());
                return found;
            }
            
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
            component.parent.SendMessage("setDirection", new Vector2(direction.x, direction.z));

            float distanceSquared = direction.LengthSquared();
            if (distanceSquared <= 30 * 30)
            {
                GD.Print(component.body.GetName(), ": Close enough to send request. Sending!");
                SetState(BreedState.WaitingForResponse);
                ((Entity)target.GetNode("Entity")).SendMessage("breedingRequest", component.body);                
            }
        }
        else if (state == BreedState.WaitingForResponse)
        {         
            component.parent.SendMessage("setDirection", new Vector2(0, 0));
            waitingTimer += delta;
            if (waitingTimer > waitingThreshold)
            {
                GD.Print(component.body.GetName(), ": Detected rejection. Idling.");    
                // Must have been rejected!
                active = false;
            }
        }
        else if (state == BreedState.GoingForBreed)
        {
            Vector3 direction = target.GetTranslation() - component.body.GetTranslation();
            component.parent.SendMessage("setDirection", new Vector2(direction.x, direction.z));
        }
        else if (state == BreedState.Breeding)
        {
            Vector3 direction = target.GetTranslation() - component.body.GetTranslation();
            component.parent.SendMessage("setDirection", new Vector2(direction.x, direction.z));
            breedingTimer += delta;

            if (breedingTimer > breedingThreshold + 1.0f)
            {
                // partner must have been eaten...
                GD.Print(component.parent.GetName(), ": Where did you go? :(");
                active = false;
            }
        }
    }
}
