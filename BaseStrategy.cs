using Godot;
using System;
using System.Collections.Generic;

public abstract class BaseStrategy
{
    public BaseStrategy(AnimalBehaviourComponent component)
    {
        this.component = component;
    }

    protected AnimalBehaviourComponent component;

    public abstract void Ready();
    public virtual void StartState(params object[] args)
    {
        active = true;
    }
    public abstract void PhysicsProcess(float delta);
    public abstract void Process(float delta);

    public virtual List<Tuple<string, object[]>> GetInitialisationSignals() { return new List<Tuple<string, object[]>>(); }

    private bool _active;
    public bool active
    {
        get { return _active; }
        set
        {
            if (value)
            {
                component._SetActiveState(this);
            }
            else
            {
                component._DeactivateState(this);
            }
            _active = value;
        }
    }
}
