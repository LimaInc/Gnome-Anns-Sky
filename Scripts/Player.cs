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

    private readonly Vector3 INERTIA = new Vector3(0.85f, 0.85f, 1);

    private Vector3 velocity = new Vector3();

    private const float CAM_ROT_SPEED = 0.01f;
    private readonly Vector3 CAM_OFFSET = new Vector3(0, 0.4f, 0);
    private readonly Vector3 DEAD_CAM_OFFSET = new Vector3(0, -0.4f, 0);

    private const float GRAVITY = -40;

    private CollisionShape collisionShape;
    private Camera myCam;

    public GUI OpenedGUI { get; set; }

    public InventoryGUI InventoryGUI { get; private set; }
    private PlayerGUI playerGUI;

    public ItemStack ItemInHand { get; set; }

    public static Texture CURSOR = ResourceLoader.Load(Game.GUI_TEXTURE_PATH + "cursor.png") as Texture;

    private const float STATS_REFERENCE = 0.001f;
    public static IDictionary<Stats, float> DEFAULT_STAT_CHANGE = new Dictionary<Stats, float>
    {
        [Stats.AIR] = 2.0f * STATS_REFERENCE,
        [Stats.WATER] = 1.4f * STATS_REFERENCE,
        [Stats.FOOD] = 1.0f * STATS_REFERENCE,
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

    public bool Dead { get; private set; }

    public const int PLAYER_INVENTORY_COUNT = 40;

    public IDictionary<Item.ItemType, Inventory> Inventories { get; private set; }

    private Interaction interaction;
    private Base planetBase;
    private Atmosphere atmosphere;
    private Plants plants;
    private BacterialState bacteria;

    private static readonly Vector3 PLAYER_INITIAL_POSITION = new Vector3(0, 60, 0);

    public override void _Ready()
    {
        interaction = GetNode(Game.CAMERA_PATH) as Interaction;
        planetBase = GetNode(Game.PLANET_BASE_PATH) as Base;
        atmosphere = GetNode(Game.ATMOSPHERE_PATH) as Atmosphere;
        plants = GetNode(Game.PLANTS_PATH) as Plants;
        bacteria = GetNode(Game.BACTERIAL_STATE_PATH) as BacterialState;

        Input.SetCustomMouseCursor(CURSOR);
        Input.SetMouseMode(Input.MouseMode.Captured);

        collisionShape = new CollisionShape();

        // OLD BoxShape collision 
        BoxShape b = new BoxShape();
        b.SetExtents(new Vector3(Chunk.BLOCK_SIZE / 2.0f - 0.05f, Chunk.BLOCK_SIZE - 0.05f,Chunk.BLOCK_SIZE / 2.0f - 0.05f));
        collisionShape.SetShape(b);

        // CapsuleShape c = new CapsuleShape();
        // c.SetRadius(Chunk.BLOCK_SIZE / 2.0f - 0.05f);
        // c.SetHeight( Chunk.BLOCK_SIZE - 0.05f);
        // collisionShape.SetShape(c);
        // collisionShape.Rotate(new Vector3(1.0f, 0.0f, 0.0f), (float) Math.PI / 2.0f);

        this.AddChild(collisionShape);
        this.SetTranslation(PLAYER_INITIAL_POSITION);

        myCam = (Camera) this.GetChild(0);
        myCam.SetTranslation(CAM_OFFSET);
        
        playerGUI = new PlayerGUI(this);
        this.AddChild(playerGUI);

        Inventories = new Dictionary<Item.ItemType, Inventory>
        {
            [Item.ItemType.CONSUMABLE] = new Inventory(Item.ItemType.CONSUMABLE, PLAYER_INVENTORY_COUNT),
            [Item.ItemType.FOSSIL] = new Inventory(Item.ItemType.FOSSIL, PLAYER_INVENTORY_COUNT),
            [Item.ItemType.BLOCK] = new Inventory(Item.ItemType.BLOCK, PLAYER_INVENTORY_COUNT)
        };

        InventoryGUI = new InventoryGUI(this, Inventories, this);

        this.AddItem(ItemStorage.cake, 3);
        this.AddItem(ItemStorage.chocolate, 10);
        this.AddItem(ItemStorage.water, 5);
    }

    // maybe a bit hacky, TODO: think about it
    private void AddItem(Item i, int n)
    {
        Inventories[i.IType].TryAddItem(i, n);
    }

    public CollisionShape GetCollisionShape()
    {
        return this.collisionShape;
    }

    public override void _Input(InputEvent e)
    {
        if (Dead) return;

        if(OpenedGUI == null)
        {
            if (e is InputEventMouseMotion emm)
            {
                Vector3 rot = this.GetRotation();

                Vector2 rel = emm.GetRelative();

                Vector3 rotd = new Vector3(-rel.y * CAM_ROT_SPEED, -rel.x * CAM_ROT_SPEED, 0.0f);

                Vector3 targetRotation = myCam.GetRotation() + rotd;

                //Clamp x rotation between -180 and 180 degrees
                float xRot = targetRotation.x;
                xRot = Mathf.Clamp(xRot, -Mathf.PI / 2, Mathf.PI / 2);
                targetRotation = new Vector3(xRot, targetRotation.y, targetRotation.z);

                myCam.SetRotation(targetRotation);
            }
            else if (e is InputEventMouseButton iemb)
            {
                if (InputUtil.IsRighPress(iemb))
                {
                    if (this.ItemInHand == null)
                    {
                        byte b = interaction.GetBlock();
                        Block block = Game.GetBlock(b);
                        if (block is DefossiliserBlock db)
                        {
                            db.HandleInput(e, this);
                        }
                    }
                    else
                    {
                        this.HandleUseItem();
                    }
                }
                else if (InputUtil.IsLeftPress(iemb))
                {
                    byte b = interaction.RemoveBlock();
                    Item ib = ItemStorage.GetItemFromBlock(b);

                    if (ib != null)
                    {
                        bool addedToHand = false;

                        if (this.ItemInHand != null)
                        {
                            Item i = this.ItemInHand.Item;
                            if (i is ItemBlock curBlock)
                            {
                                if (curBlock.Block == b)
                                {
                                    this.ItemInHand.ChangeQuantity(1);
                                    addedToHand = true;
                                }
                            }
                        }
                        if (!addedToHand)
                        {
                            this.AddItem(ib, 1);
                        }
                    }
                }
            }
        }
    }

    public void ChangeStat(Stats stat, float v)
    {
        statistics[stat] = Mathf.Min(1, Mathf.Max(0, statistics[stat] + v));
    }

    public void Push(Vector3 v)
    {
        velocity += v;
    }

    public void Kill()
    {
        if (!DEBUG_DEATH_ENABLED) 
        {
            return;
        }

        Dead = true;

        if (OpenedGUI != null)
            this.CloseGUI();
        else
            this.RemoveChild(playerGUI);

        DeadGUI dg = new DeadGUI(this);
        this.AddChild(dg);
    }

    public void HandleUseItem()
    {
        if (this.ItemInHand == null)
            return;

        Item i = this.ItemInHand.Item;

        bool success = false;
        if (i is ItemBlock b)
        {
            success = this.interaction.PlaceBlock(b.Block);
        }
        else if (i is ItemConsumable d)
        {
            ChangeStat(d.StatToReplenish, d.StatValueChange);

            success = true;
        }
        else if (i is ItemPlant p)
        {
            IntVector3? blockPos = this.interaction.GetBlockPositionUnderCursor();
            if (blockPos.HasValue)
                success = plants.Plant(p, blockPos.Value);
        }
        else if (i is ItemBacteriaVial vial)
        {
            success = bacteria.TryGetBacteria(vial.BType, out Bacteria bacterium);
            bacterium?.AddAmt(vial.Amount);
        }

        if (success)
        {
            this.ItemInHand.ChangeQuantity(-1);
            if (this.ItemInHand.Count == 0)
            {
                this.ItemInHand = null;
            }
            this.InventoryGUI.UpdateHandSlot();
        }
    }

    private void ProcessAlive(float delta)
    {
        if (Input.IsActionJustPressed("debug_kill"))
        {
            Kill();
        }
        
        if (planetBase.IsGlobalPositionInside(GetTranslation()))
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
                this.CloseGUI();
            }
        }

        DoMovement(delta);

        CheckIfStillAlive();
    }

    private void CheckIfStillAlive()
    {
        Dead |= statistics.Values.Any(v => v <= 0) || 
                GetTranslation().z <= 0;
    }

    private void DoMovement(float delta)
    {
        velocity += new Vector3(0, GRAVITY * delta, 0);
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

            Vector3 rot = myCam.GetRotation();

            float sin = (float)Math.Sin(rot.y);
            float cos = (float)Math.Cos(rot.y);

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
        this.CloseGUI();
        gui.HandleOpen(this);
        this.AddChild(gui);
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