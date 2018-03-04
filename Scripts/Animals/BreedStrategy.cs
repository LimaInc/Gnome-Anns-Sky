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

    private const float satiatedThreshold = 60.0f;

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

        component.parent.RegisterListener("acceptBreed", BreedAccepted);
        component.parent.RegisterListener("breedingRequest", AttemptBreeding);
        component.parent.RegisterListener("objectInRange", ObjectInRange);
        component.parent.RegisterListener("objectOutOfRange", ObjectOutOfRange);
        component.parent.RegisterListener("collided", Collided);
    }

    public void ObjectInRange(object[] args)
    {
        PhysicsBody body = (PhysicsBody)args[0];
        if (body.IsInGroup("animals"))
        {
            AnimalBehaviourComponent behaviourComponent = ((Entity)body.GetNode("Entity")).GetComponent<AnimalBehaviourComponent>();
            if (behaviourComponent.PresetName.Equals(component.PresetName) && behaviourComponent.Sex != component.Sex)
            {
                breedableTargets.Add(body);
            }
        }
    }

    public void ObjectOutOfRange(object[] args)
    {
        PhysicsBody body = (PhysicsBody)args[0];
        if (body.IsInGroup("animals"))
        {
            AnimalBehaviourComponent behaviourComponent = ((Entity)body.GetNode("Entity")).GetComponent<AnimalBehaviourComponent>();
            if (behaviourComponent.PresetName.Equals(component.PresetName) && behaviourComponent.Sex != component.Sex)
            {
                breedableTargets.Remove(body);
            }
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
                        if (component.Sex == AnimalBehaviourComponent.AnimalSex.Female)
                        {
                            Node spawnNode = component.parent.GetNode(Game.ANIMAL_SPAWNER_PATH);
                            int nextSex = BaseComponent.random.Next(0, 2);
                            spawnNode.Call("SpawnAnimal", component.PresetName, (AnimalBehaviourComponent.AnimalSex)nextSex, component.Body.GetTranslation() + new Vector3(0.0f, 2.0f, 0.0f));
                            component.Satiated -= component.BirthDrop;

                            //quick and dirty balance fix - frogs not reproducing enough
                            if(component.PresetName == "frog")
                            {
                                spawnNode.Call("SpawnAnimal", component.PresetName, (AnimalBehaviourComponent.AnimalSex)nextSex, component.Body.GetTranslation() + new Vector3(0.5f, 2.0f, 0.0f));
                            }
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
        SetState(BreedState.ApproachingTarget);
        target = (PhysicsBody)args[0];
        targetName = target.GetName();
    } 

    private void AttemptBreeding(object[] args)
    {
        KinematicBody otherBody = (KinematicBody)args[0];
        if (otherBody.GetName() == targetName)
        {
            ((Entity)otherBody.GetNode("Entity")).SendMessage("acceptBreed");
            SetState(BreedState.GoingForBreed);
        }
        else
        {
            if (!active || state == BreedState.ApproachingTarget)
            {
                if (component.Satiated < satiatedThreshold) return;
                int n = BaseComponent.random.Next(0, 100);
                if (n < component.Breedability)
                {
                    ((Entity)otherBody.GetNode("Entity")).SendMessage("acceptBreed");
                    target = otherBody;
                    targetName = otherBody.GetName();
                    active = true;

                    SetState(BreedState.GoingForBreed);
                }
            }
        }
    }

    private void BreedAccepted(object[] args)
    {
        SetState(BreedState.GoingForBreed);
    } 

    public override void Process(float delta)
    {
        breedableTargets.RemoveAll(p => (p == null || !p.IsInGroup("alive")));
    }

    public PhysicsBody ShouldBreedState()
    {
        if(component.Satiated > satiatedThreshold)
        {
            //only attempt breed if there is a breedable target in sight
            PhysicsBody found = null;

            foreach (PhysicsBody body in breedableTargets)
            {
                if (body == null || !body.IsInGroup("alive")) continue;
                // Identify whether we can see the target by raycasting
                PhysicsDirectSpaceState spaceState = body.GetWorld().GetDirectSpaceState();
                var result = spaceState.IntersectRay(component.Body.GetTranslation(), body.GetTranslation(), new[] { component.Body, body });

                if (result.Count == 0)
                {
                    found = body;
                    break;
                }
            }

            if (found == null) return null;

            int n = BaseComponent.random.Next(0, 100);

            if (n < component.Breedability)
            {
                return found;
            }
            
        }
        return null;
    }

    public override void PhysicsProcess(float delta)
    {
        if(target == null || !target.IsInGroup("alive")) //Still sometimes errors. Context switches?
        {
            active = false;
            return;
        }

        if (state == BreedState.ApproachingTarget)
        {
            Vector3 direction = target.GetTranslation() - component.Body.GetTranslation();
            component.parent.SendMessage("setDirection", new Vector2(direction.x, direction.z));

            float distanceSquared = direction.LengthSquared();
            if (distanceSquared <= 30 * 30)
            {
                SetState(BreedState.WaitingForResponse);
                ((Entity)target.GetNode("Entity")).SendMessage("breedingRequest", component.Body);                
            }
        }
        else if (state == BreedState.WaitingForResponse)
        {       
            component.parent.SendMessage("setDirection", new Vector2(0, 0));
            waitingTimer += delta;
            if (waitingTimer > waitingThreshold)
            { 
                // Must have been rejected!
                active = false;
            }
        }
        else if (state == BreedState.GoingForBreed)
        {
            Vector3 direction = target.GetTranslation() - component.Body.GetTranslation();
            component.parent.SendMessage("setDirection", new Vector2(direction.x, direction.z));
        }
        else if (state == BreedState.Breeding)
        {
            Vector3 direction = target.GetTranslation() - component.Body.GetTranslation();
            component.parent.SendMessage("setDirection", new Vector2(direction.x, direction.z));
            breedingTimer += delta;

            if (breedingTimer > breedingThreshold + 1.0f)
            {
                // partner must have been eaten...
                active = false;
            }
        }
    }
}
