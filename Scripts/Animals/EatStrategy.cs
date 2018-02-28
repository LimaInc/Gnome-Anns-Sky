using Godot;
using System;
using System.Collections.Generic;

public class EatStrategy : BaseStrategy
{
    public EatStrategy(AnimalBehaviourComponent component) : base(component)
    {

    }
    public enum Diet
    {
        Herbivore = 0,
        Carnivore = 1,
        Omnivore = 2
    }

    private List<PhysicsBody> foodInRange;
    private PhysicsBody target;

    public override void StartState(params object[] args)
    {
        base.StartState(args);
        target = (PhysicsBody)args[0];
    }

    public override void Ready()
    {
        foodInRange = new List<PhysicsBody>();

        component.parent.RegisterListener("collided", Collided);
        component.parent.RegisterListener("objectInRange", ObjectInRange);
        component.parent.RegisterListener("objectOutOfRange", ObjectOutOfRange);
    }

    private bool IsFood(PhysicsBody n)
    {
        switch ((Diet)component.diet)
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

    public void ObjectInRange(object[] args)
    {
        PhysicsBody otherBody = (PhysicsBody)args[0];
        AnimalBehaviourComponent behaviourComponent = ((Entity)otherBody.GetNode("Entity")).GetComponent<AnimalBehaviourComponent>();

        if (IsFood(otherBody) && behaviourComponent.foodChainLevel < component.foodChainLevel)
        {
            foodInRange.Add(otherBody);
        }
    }

    public void ObjectOutOfRange(object[] args)
    {
        PhysicsBody otherBody = (PhysicsBody)args[0];
        AnimalBehaviourComponent behaviourComponent = ((Entity)otherBody.GetNode("Entity")).GetComponent<AnimalBehaviourComponent>();

        if (IsFood(otherBody) && behaviourComponent.foodChainLevel < component.foodChainLevel)
        {
            //List is implemented as an array, so could get expensive. Could change to HashMap.
            foodInRange.Remove(otherBody);
        }
    }
    private void eat(Godot.Object nom)
    {
        component.satiated = Math.Max(100.0f, component.satiated + 20.0f);
        if (nom is PhysicsBody)
        {
            PhysicsBody otherBody = (PhysicsBody)nom;
            AnimalBehaviourComponent behaviourComponent = ((Entity)otherBody.GetNode("Entity")).GetComponent<AnimalBehaviourComponent>();
            behaviourComponent.Kill();
        }
        active = false;
    }

    public PhysicsBody ShouldEatState()
    {
        float minDistanceSquared = 1000.0f * 1000.0f;
        PhysicsBody minTarget = null;
        bool found = false;
        foreach (PhysicsBody b in foodInRange) //Could randomise this?
        {
            if (b == null) continue;
            // Identify whether we can see the target by raycasting
            PhysicsDirectSpaceState spaceState = component.body.GetWorld().GetDirectSpaceState();
            var result = spaceState.IntersectRay(component.body.GetTranslation(), b.GetTranslation(), new[] { component.body, b });

            if (result.Count == 0)
            {
                found = true;
                float distanceSquared = component.body.GetTranslation().DistanceSquaredTo(b.GetTranslation());
                if (distanceSquared < minDistanceSquared)
                {
                    minDistanceSquared = distanceSquared;
                    minTarget = b;
                }
            }
        }

        if (found)
        {
            return minTarget;
        }
        return null;
    }

    public void Collided(object[] args)
    {
        if (active)
        {
            KinematicCollision collision = (KinematicCollision)args[0];

            if (target != null && collision.Collider.Equals(target))
            {
                eat(collision.Collider);
            }
        }
    }

    public override void Process(float delta)
    {
        foodInRange.RemoveAll(p => p == null);
    }

    public override void PhysicsProcess(float delta)
    {
        if (target == null || !target.IsInGroup("alive"))
        {
            active = false;
        }
        else
        {
            // Check for line of sight and whether object is still in range.
            PhysicsDirectSpaceState spaceState = target.GetWorld().GetDirectSpaceState();
            var result = spaceState.IntersectRay(target.GetTranslation(), component.body.GetTranslation(), new[] { component.body, target });
            float distanceSquared = target.GetTranslation().DistanceSquaredTo(component.body.GetTranslation());
            if (result.Count != 0 || distanceSquared > 60 * 60)
            {
                active = false;
            }
            else
            {
                Vector3 direction = target.GetTranslation() - component.body.GetTranslation();
                component.parent.SendMessage("setDirection", new Vector2(direction.x, direction.z));
            }
        }
    }

}
