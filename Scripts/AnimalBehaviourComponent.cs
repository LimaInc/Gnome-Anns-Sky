using Godot;
using System;
using System.Collections.Generic;

public class AnimalBehaviourComponent : BaseComponent
{
    public AnimalBehaviourComponent(Entity parent, Sex sex, Diet diet, int foodChainLevel,
        int breedability, string presetName) : base(parent)
    {
        this.sex = sex;
        this.diet = diet;
        this.foodChainLevel = foodChainLevel;
        this.presetName = presetName;
        this.breedability = breedability;
        this.satiated = 80.0f;
    }

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

    public Sex sex { get; private set; }
    public Diet diet { get; private set; }
    public int foodChainLevel { get; private set; }
    public float satiated { get; set; }
    public int breedability { get; private set; }
    public string presetName { get; private set; }

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

    private void OnTerrainInterference(object[] args)
    {
        parent.SendMessage("jump", jumpMagnitude);
    }

    public override void Ready()
    {
        body = (KinematicBody)parent.GetParent();

        strategies = new List<BaseStrategy>();
        eatStrategy = new EatStrategy(this);
        breedStrategy = new BreedStrategy(this);
        strategies.Add(eatStrategy);
        strategies.Add(breedStrategy);

        state = null;

        parent.RegisterListener("terrainInterference", OnTerrainInterference);

        body.AddToGroup("animals");

        GD.Print("Animal sex: ", ((Sex)sex).ToString());

        foreach(BaseStrategy strategy in strategies)
        {
            strategy.Ready();
        }
    }

    protected void SetupInitialisationSignals()
    {
        parent.SendMessage("setSpeed", 150.0f);
        parent.SendMessage("watchFor", "plants");
        parent.SendMessage("watchFor", "animals");

        foreach(BaseStrategy strategy in strategies)
        {
            foreach(var t in strategy.GetInitialisationSignals())
            {
                parent.SendMessage(t.Item1, t.Item2);
            }
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
        parent.SendMessage("setDirection", d);
    }

    public override void Process(float delta)
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
        };
 
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

    public override void PhysicsProcess(float delta)
    {
        if (state != null)
        {
            state.PhysicsProcess(delta);
        }

        if(state == null)
        {;
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
