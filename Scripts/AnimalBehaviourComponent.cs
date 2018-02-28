using Godot;
using System;
using System.Collections.Generic;

public class AnimalBehaviourComponent : BaseComponent
{
    public enum Sex
    {
        Male = 0,
        Female = 1,
    }

    public enum Diet
    {
        Herbivore = 0,
        Carnivore = 1,
        Omnivore = 2
    }

    public enum BehaviourState
    {
        Idle = 0,
        Hunting = 1,
        Breeding = 2,
    }

    private float jumpMagnitude = 5.0f;

    private const float directionThreshold = 2.0f;
    private float directionTimer = 0.0f;

    private const float breedingThreshold = 5.0f;
    private float breedingTimer = 0.0f;

    private const float satiatedBreedThreshold = 50.0f;

    private int frameCount = 0;

    private List<BaseStrategy> strategies;
    private EatStrategy eatStrategy;
    private BreedStrategy breedStrategy;
    private BaseStrategy state;

    public KinematicBody body { get; private set; }

    private Dictionary<string, Action> registeredMethods;

    //Godot doesn't seem to support Export with getters/setters, so have to use this workaround.

    [Export]
    private int _sex;
    public int sex
    {
        get { return _sex; }
        set { _sex = value; }
    }

    [Export]
    private int _diet;
    public int diet
    {
        get { return _diet; }
        set { _diet = value; }
    }

    [Export]
    private int _foodChainLevel;
    public int foodChainLevel
    {
        get { return _foodChainLevel; }
        set { _foodChainLevel = value; }
    }

    [Export]
    private float _satiated = 80.0f;
    public float satiated {
        get { return _satiated; }
        set { _satiated = value; }
    }

    [Export]
    private int _breedability;
    public int breedability
    {
        get { return _breedability; }
        set { _breedability = value; }
    }

    [Export]
    private string _presetName;
    public string presetName
    {
        get { return _presetName; }
        set { _presetName = value; }
    }

    const float timeToDeath = 50.0f;

    public void _SetActiveState(BaseStrategy strategy)
    {
        if(state != null)
        {
            state.active = false;
        }
        state = strategy;     
    }

    public void _DeactivateState(BaseStrategy strategy)
    {
        state = null;
    }

    private void OnTerrainInterference()
    {
        body.EmitSignal("jump", jumpMagnitude);
    }

    public override void _Ready()
    {    
        body = (KinematicBody)GetParent();

        strategies = new List<BaseStrategy>();
        eatStrategy = new EatStrategy(this);
        breedStrategy = new BreedStrategy(this);
        strategies.Add(eatStrategy);
        strategies.Add(breedStrategy);

        customProperties = new Dictionary<string, Tuple<Node, string>>();

        state = null;

        body.AddUserSignal("jump");
        body.AddUserSignal("setDirection");
        body.AddUserSignal("setSpeed");
        body.AddUserSignal("watchFor");
        body.AddUserSignal("setVisionRange");
        body.AddUserSignal("attemptBreeding");

        body.AddToGroup("animals");

        GD.Print("Animal sex: ", ((Sex)sex).ToString());

        foreach(BaseStrategy strategy in strategies)
        {
            strategy.Ready();
        }
    }

    protected void SetupInitialisationSignals()
    {
        body.EmitSignal("setSpeed", 150.0f);
        body.EmitSignal("watchFor", "plants");
        body.EmitSignal("watchFor", "animals");

        foreach(BaseStrategy strategy in strategies)
        {
            foreach(var t in strategy.GetInitialisationSignals())
            {
                body.EmitSignal(t.Item1, t.Item2);
            }
        }
    }

    protected void SetVisionRange(float range)
    {
        body.EmitSignal("setVisionRange", range);
    }

    protected void ObjectInRange(PhysicsBody n)
    {
        foreach(BaseStrategy strategy in strategies)
        {
            strategy.ObjectInRange(n);
        }
    }

    protected void ObjectOutOfRange(PhysicsBody n)
    {
        foreach (BaseStrategy strategy in strategies)
        {
            strategy.ObjectOutOfRange(n);
        }
    }

    protected void Collided(KinematicCollision collision)
    {
        if (state != null)
        {
            state.Collided(collision);
        }
    }

    protected void hungry(float delta)
    {
        satiated -= delta * (100.0f/timeToDeath);

        if(satiated <= 0.0f)
        {
            GD.Print("Starved to death!");
            body.QueueFree();
        }
    }

    private void SetRandomDirection()
    {
        Random r = new Random();
        Vector2 d = new Vector2((float)(r.NextDouble() * 2.0 - 1.0), (float)(r.NextDouble() * 2.0 - 1.0));
        //GD.Print("Direction set sent: ", d.x, " ", d.y);
        body.EmitSignal("setDirection", d);
    }

    public override void _Process(float delta)
    {
        if(frameCount == 1)
        {
            // initialise things that rely on signals only if not the first frame
            // this guarantees that other components will have set up their connections
            SetupInitialisationSignals();

            frameCount++;
        }
        else if(frameCount == 0)
        {
            frameCount++;
        }

        SetupConnection("terrainInterference", body, this, nameof(OnTerrainInterference));
        SetupConnection("objectInRange", body, this, nameof(ObjectInRange));
        SetupConnection("objectOutOfRange", body, this, nameof(ObjectOutOfRange));
        SetupConnection("collided", body, this, nameof(Collided));
        
        foreach(BaseStrategy strategy in strategies)
        {
            foreach(var t in strategy.GetConnections())
            {
                SetupConnection(t.Item1, body, t.Item2, t.Item3);
            }
        }
 
        if(state != null)
        {
            state.Process(delta);
        }

        hungry(delta);

       if (state == null)
        {
            directionTimer += delta;
            breedingTimer += delta;
            if (directionTimer > directionThreshold)
            {
                directionTimer = 0;
                SetRandomDirection();
            }
            
        }
    }

    public void RegisteredMethods(string name, Node obj, string getter)
    {
        customProperties.Add(name, Tuple.Create(obj, getter));
    }

    //Godot's "Call" functionality doesn't seem to support variable arguments correctly.
    //Force the pass of args.
    public object GetCustomProperty(string name, params object[] args)
    {
        //if (name != "breedingTargetName")
        //{
            GD.Print("GetCustomProperty called: ", name, args,args.Length);
        //}
        Tuple<Node, string> t = customProperties[name];
        return t.Item1.Call(t.Item2, args);
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);

        if(state != null)
        {
            state.PhysicsProcess(delta);
        }

        if(state == null)
        {
            if(breedingTimer > breedingThreshold && satiated > satiatedBreedThreshold)
            {
                PhysicsBody breedTarget = breedStrategy.ShouldBreedState();
                if(breedTarget != null)
                {
                    GD.Print(body.GetName(), " Starting breed state!"); 
                    breedStrategy.StartState(breedTarget);
                }
            }else if (satiated < 80.0f)
            {
                PhysicsBody eatTarget = eatStrategy.ShouldEatState();
                if(eatTarget != null){
                    eatStrategy.StartState(eatTarget);
                }
            }
        }
    }
}
