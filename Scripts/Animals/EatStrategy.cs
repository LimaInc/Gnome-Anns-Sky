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
        switch ((Diet)component.Diet)
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

        if (IsFood(otherBody) && behaviourComponent.FoodChainLevel < component.FoodChainLevel)
        {
            foodInRange.Add(otherBody);
        }
    }

    public void ObjectOutOfRange(object[] args)
    {
        PhysicsBody otherBody = (PhysicsBody)args[0];
        AnimalBehaviourComponent behaviourComponent = ((Entity)otherBody.GetNode("Entity")).GetComponent<AnimalBehaviourComponent>();

        if (IsFood(otherBody) && behaviourComponent.FoodChainLevel < component.FoodChainLevel)
        {
            //List is implemented as an array, so could get expensive. Could change to HashMap.
            foodInRange.Remove(otherBody);
        }
    }
    private void eat(Godot.Object nom)
    {
        component.Satiated = Math.Max(100.0f, component.Satiated + 20.0f);
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
            PhysicsDirectSpaceState spaceState = component.Body.GetWorld().GetDirectSpaceState();
            var result = spaceState.IntersectRay(component.Body.GetTranslation(), b.GetTranslation(), new[] { component.Body, b });

            if (result.Count == 0)
            {
                found = true;
                float distanceSquared = component.Body.GetTranslation().DistanceSquaredTo(b.GetTranslation());
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
            var result = spaceState.IntersectRay(target.GetTranslation(), component.Body.GetTranslation(), new[] { component.Body, target });
            float distanceSquared = target.GetTranslation().DistanceSquaredTo(component.Body.GetTranslation());
            if (result.Count != 0 || distanceSquared > 60 * 60)
            {
                active = false;
            }
            else
            {
                Vector3 direction = target.GetTranslation() - component.Body.GetTranslation();
                component.parent.SendMessage("setDirection", new Vector2(direction.x, direction.z));
            }
        }
    }

}
