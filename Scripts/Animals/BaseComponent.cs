using Godot;
using System;
using System.Collections.Generic;

public abstract class BaseComponent : Godot.Object
{
    public Entity parent { get; private set; }

    public static Random random = new Random();
    public BaseComponent(Entity parent)
    {
        this.parent = parent;
    }

    public abstract void Ready();
    public abstract void Process(float delta);
    public abstract void PhysicsProcess(float delta);
}
