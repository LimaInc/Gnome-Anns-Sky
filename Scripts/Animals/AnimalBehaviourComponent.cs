using Godot;
using System;
using System.Collections.Generic;

public class AnimalBehaviourComponent : BaseComponent
{
    private static readonly byte RED_ROCK_ID = Game.GetBlockId<RedRock>();
    private static readonly byte GRASS_BLOCK_ID = Game.GetBlockId<GrassBlock>();


    Terrain terrain; 

    public AnimalBehaviourComponent(Entity parent, AnimalSex sex, AnimalDiet diet, int foodChainLevel,
        int breedability, string presetName, float oxygenConsumption, float co2Production, int foodDrop) : base(parent)
    {
        this.Sex = sex;
        this.Diet = diet;
        this.FoodChainLevel = foodChainLevel;
        this.PresetName = presetName;
        this.Breedability = breedability;
        this.Satiated = 80.0f;
        this.oxygenConsumption = oxygenConsumption;
        this.co2Production = co2Production;
        this.FoodDrop = foodDrop;
    }

    public enum AnimalSex
    {
        Male = 0,
        Female = 1,
    }

    public enum AnimalDiet
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

    public void Kill()
    {
        Body.RemoveFromGroup("alive");
        Body.QueueFree();
    }

    public bool IsAlive()
    {
        return Body.IsInGroup("alive");
    }

    private float jumpMagnitude = 5.0f;

    private const float directionThreshold = 2.0f;
    private float directionTimer = 0.0f;

    private const float breedingThreshold = 5.0f;
    private float breedingTimer = 0.0f;

    private const float satiatedBreedThreshold = 80.0f;

    private const float co2Threshold = 0.5f;
    private const float oxygenThreshold = 0.5f;

    private int frameCount = 0;

    private List<BaseStrategy> strategies;
    private EatStrategy eatStrategy;
    private BreedStrategy breedStrategy;
    private BaseStrategy state;

    public KinematicBody Body { get; private set; }

    public int FoodDrop { get; private set; }
    public AnimalSex Sex { get; private set; }
    public AnimalDiet Diet { get; private set; }
    public int FoodChainLevel { get; private set; }
    public float Satiated { get; set; }
    public int Breedability { get; private set; }
    public string PresetName { get; private set; }

    private float oxygenConsumption; //per sec
    private float co2Production; //per sec 

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
        Body = (KinematicBody)parent.GetParent();

        // Make body able to collide with grass.
        Body.SetCollisionMaskBit(1, true);

        Area area = (Area)Body.GetNode("Area");

        // Body collides with layers 0 and 1. Having body's mask match area's layer when they overlap and the area gets removed causes a Godot bug,
        // so disable that.
        area.SetCollisionLayerBit(0, false);
        area.SetCollisionLayerBit(31, true);

        strategies = new List<BaseStrategy>();
        eatStrategy = new EatStrategy(this);
        breedStrategy = new BreedStrategy(this);
        strategies.Add(eatStrategy);
        strategies.Add(breedStrategy);

        state = null;

        parent.RegisterListener("terrainInterference", OnTerrainInterference);

        Body.AddToGroup("animals");
        Body.AddToGroup("alive");

        foreach(BaseStrategy strategy in strategies)
        {
            strategy.Ready();
        }

        terrain = parent.GetNode(Game.TERRAIN_PATH) as Terrain;
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

    protected void Hungry(float delta)
    {
        Satiated -= delta * (100.0f/timeToDeath);

        if(Satiated <= 0.0f)
        {
            Kill();
        }
    }

    private void SetRandomDirection()
    {
        Vector2 d = new Vector2((float)(random.NextDouble() * 2.0 - 1.0), (float)(random.NextDouble() * 2.0 - 1.0));
        parent.SendMessage("setDirection", d);
    }

    private void DoAtmosphereEffects(float delta)
    {
        Atmosphere atmosphere = (Atmosphere)(Body.GetTree().GetRoot().GetNode(Game.ATMOSPHERE_PATH));
        if(!(atmosphere.GetGasProgress(Gas.OXYGEN) > oxygenThreshold && atmosphere.GetGasProgress(Gas.CARBON_DIOXIDE) > co2Threshold))
        {
            Kill(); 
        }
        else
        {
            atmosphere.SetGasAmt(Gas.CARBON_DIOXIDE, atmosphere.GetGasAmt(Gas.CARBON_DIOXIDE) + co2Production * delta);
            atmosphere.SetGasAmt(Gas.OXYGEN, atmosphere.GetGasAmt(Gas.OXYGEN) +- oxygenConsumption * delta);
        }
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

        Hungry(delta);
        DoAtmosphereEffects(delta);

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
        {
            if (breedingTimer > breedingThreshold && Satiated > satiatedBreedThreshold)
            {
                PhysicsBody breedTarget = breedStrategy.ShouldBreedState();
                if(breedTarget != null)
                {
                    breedStrategy.StartState(breedTarget);
                }
                breedingTimer = 0;
            }
            else if (Satiated < 80.0f)
            {
                if (Diet == AnimalDiet.Herbivore)
                {
                    //just try to eat whatever block is below

                    Vector3 pos = Body.GetTranslation();
                    IntVector3 blockPos = new IntVector3((int)Mathf.Round(pos.x / Block.SIZE), (int)Mathf.Round(pos.y / Block.SIZE), (int)Mathf.Round(pos.z / Block.SIZE));
                    blockPos.y--;
                    byte block = terrain.GetBlock(blockPos);
                    if(block == GRASS_BLOCK_ID)
                    {
                        GD.Print("Eat grass!");
                        Satiated = Math.Max(100.0f, Satiated + 20.0f);
                        terrain.SetBlock(blockPos,RED_ROCK_ID);
                    }
                }
                else
                {
                    PhysicsBody eatTarget = eatStrategy.ShouldEatState();
                    if (eatTarget != null)
                    {
                        eatStrategy.StartState(eatTarget);
                    }
                }
            }
        }
    }
}
