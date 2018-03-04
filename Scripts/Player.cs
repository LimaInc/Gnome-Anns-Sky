using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Player : KinematicBody
{
    public enum Stats
    {
        AIR, WATER, FOOD
    }

    private static bool DEBUG_DEATH_ENABLED = false;

    private const float MOVE_SPEED = 30;
    private const float JUMP_POWER = 12;
    private const float SPRINT_SPEED = MOVE_SPEED * SPRINT_MULT;

    private const float SPRINT_MULT = 3;

    private readonly Vector3 INERTIA = new Vector3(0.85f, 1, 0.85f);

    private Vector3 velocity = new Vector3();

    private const float CAM_ROT_SPEED = 0.01f;
    private readonly Vector3 CAM_OFFSET = new Vector3(0, 0.4f, 0);
    private readonly Vector3 DEAD_CAM_OFFSET = new Vector3(0, -0.4f, 0);

    private readonly Vector3 GRAVITY = new Vector3(0, -40, 0);

    private CollisionShape collisionShape;
    private Camera myCam;

    public GUI OpenedGUI { get; set; }

    public InventoryGUI InventoryGUI { get; private set; }
    private PlayerGUI playerGUI;

    public ItemStack ItemInHand { get; set; }

    public static Texture CURSOR = ResourceLoader.Load(Game.GUI_TEXTURE_PATH + "cursor.png") as Texture;

    private const float STATS_REFERENCE = 0.0015f;
    public static IDictionary<Stats, float> DEFAULT_STAT_CHANGE = new Dictionary<Stats, float>
    {
        [Stats.AIR] = - 2.0f * STATS_REFERENCE,
        [Stats.WATER] = - 1.4f * STATS_REFERENCE,
        [Stats.FOOD] = - 1.0f * STATS_REFERENCE,
    };
    private const float MOVE_DEGRAD_MULT = 5;
    private const float SPRINT_DEGRAD_MULT = 4;
    private const float BASE_AIR_REGEN_MULT = 25;
    private const float ATM_AIR_MAX_REGEN_MULT = 20;
    //single frame
    private const float JUMP_DEGRAD_MULT = 50;

    private const float BASE_AIR_REGEN = BASE_AIR_REGEN_MULT * STATS_REFERENCE;
    private const float ATMOSPHERE_AIR_REGEN = ATM_AIR_MAX_REGEN_MULT * STATS_REFERENCE;

    private readonly IDictionary<Stats, float> statistics = new Dictionary<Stats, float>
    {
        [Stats.AIR] = 1,
        [Stats.WATER] = 1,
        [Stats.FOOD] = 1
    };

    private bool dead = false;
    public bool Dead {
        get => dead;
        private set
        {
            if (!dead && value && DEBUG_DEATH_ENABLED)
            {
                OpenGUI(new DeadGUI(this));
            }
            dead = value;
        }
    }

    public const int PLAYER_INVENTORY_COUNT = 40;

    public IDictionary<Item.ItemType, Inventory> Inventories { get; private set; }

    private Interaction interaction;
    private Base planetBase;
    private Atmosphere atmosphere;
    private Plants plants;
    private BacterialState bacteria;

    public static readonly Vector3 INIT_REL_POS = new Vector3(0, 5, 0);

    public override void _Ready()
    {
        interaction = GetNode(Game.CAMERA_PATH) as Interaction;
        planetBase = GetNode(Game.PLANET_BASE_PATH) as Base;
        atmosphere = GetNode(Game.ATMOSPHERE_PATH) as Atmosphere;
        plants = GetNode(Game.PLANTS_PATH) as Plants;
        bacteria = GetNode(Game.BACTERIAL_STATE_PATH) as BacterialState;

        Input.SetCustomMouseCursor(CURSOR);
        Input.SetMouseMode(Input.MouseMode.Captured);

        Area area = new Area
        {
            Name = "AnimalArea"
        };
        area.AddChild(new CollisionShape
        {
            Shape = new BoxShape
            {
                Extents = (Chunk.SIZE * Block.SIZE / 2) * Terrain.ANIMAL_CHUNK_RANGE
            }
        });
        AddChild(area);

        // OLD BoxShape collision 
        collisionShape = new CollisionShape
        {
            Shape = new BoxShape
            {
                Extents = new Vector3(Block.SIZE / 2 - 0.05f, Block.SIZE - 0.05f, Block.SIZE / 2 - 0.05f)
            }
        };
        // CapsuleShape c = new CapsuleShape();
        // c.SetRadius(Block.SIZE / 2.0f - 0.05f);
        // c.SetHeight( Block.SIZE - 0.05f);
        // collisionShape.SetShape(c);
        // collisionShape.Rotate(new Vector3(1.0f, 0.0f, 0.0f), (float) Math.PI / 2.0f);

        AddChild(collisionShape);

        myCam = GetNode(Game.CAMERA_PATH) as Camera;
        myCam.Translation = CAM_OFFSET;
        
        // no idea about the trig of camera rotation, TODO: figure it out
        playerGUI = new PlayerGUI(this, 
            () => new Vector2(Mathf.Sin(myCam.Rotation.y), Mathf.Cos(myCam.Rotation.y)),
            new Vector2(planetBase.position.x, planetBase.position.z));
        AddChild(playerGUI);

        Inventories = new Dictionary<Item.ItemType, Inventory>
        {
            [Item.ItemType.CONSUMABLE] = new Inventory(Item.ItemType.CONSUMABLE, PLAYER_INVENTORY_COUNT),
            [Item.ItemType.PROCESSED] = new Inventory(Item.ItemType.PROCESSED, PLAYER_INVENTORY_COUNT),
            [Item.ItemType.BLOCK] = new Inventory(Item.ItemType.BLOCK, PLAYER_INVENTORY_COUNT)
        };

        InventoryGUI = new InventoryGUI(this, Inventories, this);

        this.AddItem(ItemStorage.items[ItemID.CAKE], 3);
        this.AddItem(ItemStorage.items[ItemID.CHOCOLATE], 10);
        this.AddItem(ItemStorage.items[ItemID.WATER], 5);

        //TEMP
        this.AddItem(ItemStorage.items[ItemID.CARBON_DIOXIDE_BACTERIA_VIAL], 100);
        this.AddItem(ItemStorage.items[ItemID.OXYGEN_BACTERIA_VIAL], 100);
        this.AddItem(ItemStorage.items[ItemID.NITROGEN_BACTERIA_VIAL], 100);
        this.AddItem(ItemStorage.items[ItemID.REGULAR_EGG], 100);
        this.AddItem(ItemStorage.items[ItemID.BIG_EGG], 100);
        this.AddItem(ItemStorage.items[ItemID.FROG_EGG], 100);
        this.AddItem(ItemStorage.items[ItemID.GRASS], 100);
        this.AddItem(ItemStorage.items[ItemID.TREE], 100);
        this.AddItem(ItemStorage.items[ItemID.WHEAT], 100);

    }

    // maybe a bit hacky, TODO: think about it
    public void AddItem(Item i, int n)
    {
        Inventories[i.IType].TryAddItem(i, n);
    }

    public override void _Input(InputEvent e)
    {
        if (Dead)
        {
            return;
        }

        if(OpenedGUI == null)
        {
            if (e is InputEventMouseMotion emm)
            {
                Vector3 targetRotation = myCam.Rotation + 
                    new Vector3(-emm.Relative.y * CAM_ROT_SPEED, -emm.Relative.x * CAM_ROT_SPEED, 0.0f);

                //Clamp x rotation between -180 and 180 degrees
                targetRotation = new Vector3(
                    Mathf.Clamp(targetRotation.x, -Mathf.PI / 2, Mathf.PI / 2),
                    targetRotation.y,
                    targetRotation.z);

                myCam.Rotation = targetRotation;
            }
            else if (e is InputEventMouseButton iemb)
            {
                if (InputUtil.IsRighPress(iemb))
                {
                    if (ItemInHand == null)
                    {
                        byte b = interaction.GetBlock(interaction.GetHitInfo());
                        Block block = Game.GetBlock(b);
                        if (block is DefossiliserBlock db)
                        {
                            db.HandleInput(e, this);
                        }
                    }
                    else
                    {
                        HandleUseItem();
                    }
                }
                else if (InputUtil.IsLeftPress(iemb))
                {
                    byte b = interaction.RemoveBlock();
                    Item ib = ItemStorage.GetItemFromBlock(b);

                    if (ib != null)
                    {
                        if (ItemInHand == null)
                        {
                            ItemInHand = new ItemStack(ItemStorage.GetItemFromBlock(b), 1);
                        }
                        else
                        {
                            Item i = ItemInHand.Item;
                            if (i is ItemBlock curBlock)
                            {
                                if (curBlock.Block == b)
                                {
                                    ItemInHand.ChangeQuantity(1);
                                }
                                else
                                {
                                    AddItem(ib, 1);
                                }
                            }
                            else
                            {
                                AddItem(ib, 1);
                            }
                        }
                    }
                }
            }
        }
    }

    public void ChangeStat(Stats stat, float v)
    {
        statistics[stat] = Mathf.Clamp(statistics[stat] + v, 0, 1);
    }

    public void Push(Vector3 v)
    {
        velocity += v;
    }

    public void HandleUseItem()
    {
        if (ItemInHand == null)
            return;

        Item i = this.ItemInHand.Item;

        bool success = false;
        if (i is ItemBlock b)
        {
            success = interaction.PlaceBlock(b.Block);
        }
        else if (i is ItemConsumable d)
        {
            ChangeStat(d.StatToReplenish, d.StatValueChange);

            success = true;
        }
        else if (i is ItemBacteriaVial vial)
        {
            success = bacteria.TryGetBacteria(vial.BType, out Bacteria bacterium);
            bacterium?.AddAmt(vial.Amount);
        }
        else if (i is ItemPlant p)
        {
            IntVector3? blockPos = interaction.GetBlockPositionUnderCursor(interaction.GetHitInfo());
            if (blockPos.HasValue)
                success = plants.Plant(p, blockPos.Value);
        }
        else if (i is ItemSpawnEgg egg)
        {
            Vector3? graphicalPosition = (Vector3?)interaction.GetHitInfo()?["position"];
            if (graphicalPosition.HasValue)
            {
                string animal = egg.Preset;
                int nextSex = BaseComponent.random.Next(0, 2);
                GetNode(Game.ANIMAL_SPAWNER_PATH).Call("SpawnAnimal", animal, (AnimalBehaviourComponent.AnimalSex)nextSex, graphicalPosition.Value + new Vector3(0,2,0));
                success = true;
            }
                
        }

        if (success)
        {
            ItemInHand.ChangeQuantity(-1);
            if (ItemInHand.Count == 0)
            {
                ItemInHand = null;
            }
        }
    }

    private void ProcessAlive(float delta)
    {
        if (Input.IsActionJustPressed("debug_kill"))
        {
            Dead = true;
            return;
        }
        
        if (planetBase.IsGlobalPositionInside(Translation))
        {
            ChangeStat(Stats.AIR, BASE_AIR_REGEN * delta);
        }
        ChangeStat(Stats.AIR, ATMOSPHERE_AIR_REGEN * delta * Mathf.Pow(atmosphere.GetGasProgress(Gas.OXYGEN),2));
        foreach(KeyValuePair<Stats,float> kvPair in DEFAULT_STAT_CHANGE)
        {
            ChangeStat(kvPair.Key, delta * kvPair.Value);
        }

        if (Input.IsActionJustPressed("inventory"))
        {
            if (OpenedGUI == null)
            {
                OpenGUI(InventoryGUI);
            }
            else
            {
                CloseGUI();
            }
        }

        DoMovement(delta);

        CheckIfStillAlive();
    }

    private void CheckIfStillAlive()
    {
        if (DEBUG_DEATH_ENABLED && !Dead)
        {
            Dead = statistics.Values.Any(v => v <= 0) || Translation.y <= 0;
        }
    }

    private void DoMovement(float delta)
    {
        velocity += GRAVITY * delta;
        if (OpenedGUI == null)
        {
            if (Input.IsActionJustPressed("jump") && IsOnFloor())
            {
                foreach (KeyValuePair<Stats, float> kvPair in DEFAULT_STAT_CHANGE)
                {
                    ChangeStat(kvPair.Key, delta * JUMP_DEGRAD_MULT * kvPair.Value);
                }

                velocity += new Vector3(0.0f, JUMP_POWER, 0.0f);
            }

            float sin = Mathf.Sin(myCam.Rotation.y);
            float cos = Mathf.Cos(myCam.Rotation.y);

            Vector3 movDir = new Vector3();
            if (Input.IsActionPressed("forward"))
            {
                movDir += new Vector3(-sin, 0.0f, -cos);
            }
            if (Input.IsActionPressed("backward"))
            {
                movDir += new Vector3(sin, 0.0f, cos);
            }
            if (Input.IsActionPressed("left"))
            {
                movDir += new Vector3(-cos, 0.0f, sin);
            }
            if (Input.IsActionPressed("right"))
            {
                movDir += new Vector3(cos, 0.0f, -sin);
            }

            if (!movDir.Equals(new Vector3()))
            {
                float speed;
                float degradMult;
                if (Input.IsActionPressed("sprint"))
                {
                    degradMult = SPRINT_DEGRAD_MULT;
                    speed = SPRINT_SPEED;
                }
                else
                {
                    degradMult = MOVE_DEGRAD_MULT;
                    speed = MOVE_SPEED;
                }
                foreach (KeyValuePair<Stats, float> kvPair in DEFAULT_STAT_CHANGE)
                {
                    ChangeStat(kvPair.Key, degradMult * delta * kvPair.Value);
                }
                movDir = movDir.Normalized();
                velocity += movDir * speed * delta; ;
            }
        }
        velocity *= INERTIA;
        velocity = MoveAndSlide(velocity, new Vector3(0, 1, 0));
    }

    public void OpenGUI(GUI gui)
    {
        CloseGUI();
        gui.HandleOpen(this);
        AddChild(gui);
        OpenedGUI = gui;
        playerGUI.BackgroundMode = true;
    }

    public void CloseGUI()
    {
        if (OpenedGUI != null)
        {
            OpenedGUI.HandleClose();
            this.RemoveChild(OpenedGUI);
            OpenedGUI = null;
            playerGUI.BackgroundMode = false;
        }
    }

    public void ProcessDead(float delta)
    {
    }

    public override void _Process(float delta)
    {
        if (Dead)
        {
            ProcessDead(delta);
        } else
        {
            ProcessAlive(delta);
        }
    }

    public float this[Stats s]
    {
        get => statistics[s];
    }
}