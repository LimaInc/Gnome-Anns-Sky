using Godot;
using System;
using System.Collections.Generic;

public class EatStrategy : BaseStrategy
{
    EatStrategy(AnimalBehaviourComponent component) : base(component)
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

    public override void ObjectInRange(PhysicsBody otherBody)
    {
        base.ObjectInRange(otherBody);

        Node behaviourComponent = otherBody.GetNode("behaviourComponent");
        if (IsFood(otherBody) && (int)behaviourComponent.Get("foodChainLevel") < component.foodChainLevel)
        {
            foodInRange.Add(otherBody);
        }
    }

    public override void ObjectOutOfRange(PhysicsBody otherBody)
    {
        base.ObjectOutOfRange(otherBody);

        Node behaviourComponent = otherBody.GetNode("BehaviourComponent");
        if (IsFood(otherBody) && (int)behaviourComponent.Get("foodChainLevel") < component.foodChainLevel)
        {
            //List is implemented as an array, so could get expensive. Could change to HashMap.
            foodInRange.Remove(otherBody);
        }
    }
    private void eat(Godot.Object nom)
    {
        GD.Print("Nom!");
        component.satiated = Math.Max(100.0f, component.satiated + 20.0f);
        if (nom is Node)
        {
            ((Node)nom).QueueFree();
        }
        active = false;
    }

    public override void Collided(KinematicCollision collision)
    {
        base.Collided(collision);

        if(target != null && collision.Collider.Equals(target))
        {
            eat(collision.Collider);
        }
    }

    public override void Process(float delta)
    {
        foodInRange.RemoveAll(p => p == null);
    }

    public override void PhysicsProcess(float delta)
    {
        if(target == null)
        {
            active = false;
        }
        else
        {
            // Check for line of sight and whether object is still in range.
            PhysicsDirectSpaceState spaceState = target.GetWorld().GetDirectSpaceState();
            var result = spaceState.IntersectRay(target.GetTranslation(), parent.GetTranslation(), new[] { parent, target });
            float distanceSquared = target.GetTranslation().DistanceSquaredTo(parent.GetTranslation());
            if (result.Count != 0 || distanceSquared > 60 * 60)
            {
                state = BehaviourState.Idle;
            }
            else
            {
                Vector3 direction = target.GetTranslation() - parent.GetTranslation();
                parent.EmitSignal("setDirection", new Vector2(direction.x, direction.z));
            }
        }
    }

}
