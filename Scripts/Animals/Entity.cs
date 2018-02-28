using Godot;
using System.Collections.Generic;
using System;

public class Entity : Node
{
    private List<BaseComponent> components;

    public Entity() : base()
    {
        components = new List<BaseComponent>();
        listeners = new Dictionary<string, List<ParamsAction>>();
    }

    public void AddComponent(BaseComponent component)
    {
        components.Add(component);
    }

    public override void _Ready()
    {
        components.ForEach(c => c.Ready());
    }

    public override void _Process(float delta)
    {
        base._Process(delta);

        components.ForEach(c => c.Process(delta));
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);

        components.ForEach(c => c.PhysicsProcess(delta));
    }

    public T GetComponent<T> () where T: BaseComponent
    {
        foreach(BaseComponent c in components)
        {
            if(c.GetType() == typeof(T))
            {
                return (T)c;
            }
        }
        return null;
    }

    //Primitive messaging system.
    //Could be extended to be made much nicer (with no strings) and type safe.
    public delegate void ParamsAction(params object[] args);

    private Dictionary<string, List<ParamsAction>> listeners;

    //Any object can listen to an event any other object receives by design.
    public void RegisterListener(string name, ParamsAction action)
    {
        if (listeners.ContainsKey(name))
        {
            listeners[name].Add(action);
        }
        else
        {
            listeners.Add(name, new List<ParamsAction>() { action });
        }
    }

    public void SendMessage(string name, params object[] args)
    {
        if (listeners.ContainsKey(name))
        {
            List<ParamsAction> list = listeners[name];
            foreach (ParamsAction action in list)
            {
                action(args);
            }
        }
    }
}
