using Godot;
using System;
using System.Collections.Generic;

public class Player : KinematicBody
{
    private static bool DEBUG_DEATH_ENABLED = false;

    private float moveSpeed = 30.0f;
    private float jumpPower = 12.0f;
    private float camRotateSpeed = 0.01f;

    private float xz_inertia = 0.85f;
    private float y_intertia = 1;

    private Vector3 velocity = new Vector3();

    private Vector3 camOffset = new Vector3(0.0f, 0.4f, 0.0f);
    private Vector3 deadCamOffset = new Vector3(0.0f, -0.4f, 0.0f);

    private static float gravity = 40.0f;

    private CollisionShape collisionShape;
    private Camera myCam;

    public GUI OpenedGUI { get; set; }

    public InventoryGUI InventoryGUI { get; private set; }
    private PlayerGUI playerGUI;

    public ItemStack ItemInHand { get; set; }

    public static Texture CURSOR = ResourceLoader.Load(Game.GUI_TEXTURE_PATH + "cursor.png") as Texture;

    //Original implementation was written in integers, hence why the max constants exist
    public static float MAX_AIR = 1.0f;
    public float CurrentAir { get; set; } = MAX_AIR;

    public static float MAX_THIRST = 1.0f;
    public float CurrentThirst { get; set; } = MAX_THIRST;

    public static float MAX_HUNGER = 1.0f;
    public float CurrentHunger { get; set; } = MAX_HUNGER;

    private Interaction interaction;

    private Game game;

    private bool dead;

    public const int PLAYER_INVENTORY_COUNT = 40;

    public IDictionary<Item.Type, Inventory> Inventories { get; private set; }
    private Plants plants;
    private Atmosphere atmosphere;

    public override void _Ready()
    {
        game = GetNode(Game.GAME_PATH) as Game;

        Input.SetCustomMouseCursor(CURSOR);
        Input.SetMouseMode(Input.MouseMode.Captured);

        collisionShape = new CollisionShape();

        //OLD BoxShape collision 
        BoxShape b = new BoxShape();
        b.SetExtents(new Vector3(Chunk.BLOCK_SIZE / 2.0f - 0.05f, Chunk.BLOCK_SIZE - 0.05f,Chunk.BLOCK_SIZE / 2.0f - 0.05f));
        collisionShape.SetShape(b);

        interaction = GetNode(Game.CAMERA_PATH) as Interaction;

        // CapsuleShape c = new CapsuleShape();
        // c.SetRadius(Chunk.BLOCK_SIZE / 2.0f - 0.05f);
        // c.SetHeight( Chunk.BLOCK_SIZE - 0.05f);
        // collisionShape.SetShape(c);
        // collisionShape.Rotate(new Vector3(1.0f, 0.0f, 0.0f), (float) Math.PI / 2.0f);

        this.AddChild(collisionShape);
        this.SetTranslation(new Vector3(0.0f,40.0f,0.0f));

        myCam = (Camera) this.GetChild(0);
        myCam.SetTranslation(camOffset);
        
        playerGUI = new PlayerGUI(this);
        this.AddChild(playerGUI);

        plants = GetNode(Game.PLANTS_PATH) as Plants;

        Inventories = new Dictionary<Item.Type, Inventory>
        {
            [Item.Type.CONSUMABLE] = new Inventory(Item.Type.CONSUMABLE, PLAYER_INVENTORY_COUNT),
            [Item.Type.FOSSIL] = new Inventory(Item.Type.FOSSIL, PLAYER_INVENTORY_COUNT),
            [Item.Type.BLOCK] = new Inventory(Item.Type.BLOCK, PLAYER_INVENTORY_COUNT)
        };

        InventoryGUI = new InventoryGUI(this, Inventories, this);


        this.AddItem(ItemStorage.redRock, 20);
        this.AddItem(ItemStorage.redRock, 15);
        this.AddItem(ItemStorage.redRock, 34);

        this.AddItem(ItemStorage.cake, 3);
        this.AddItem(ItemStorage.chocolate, 10);
        this.AddItem(ItemStorage.water, 5);

        this.AddItem(ItemStorage.oxygenBacteriaFossil, 10);
        this.AddItem(ItemStorage.oxygenBacteriaFossil, 5);
        this.AddItem(ItemStorage.nitrogenBacteriaFossil, 15);
        this.AddItem(ItemStorage.carbonDioxideBacteriaFossil, 15);

        this.AddItem(ItemStorage.grassFossil, 10);
        this.AddItem(ItemStorage.treeFossil, 10);

        this.atmosphere = GetNode(Game.ATMOSPHERE_PATH) as Atmosphere;
    }

    private void AddItem(Item i, int n)
    {
        Inventories[i.GetType()].TryAddItem(i, n);
    }

    public CollisionShape GetCollisionShape()
    {
        return this.collisionShape;
    }

    public bool IsInventoryOpen()
    {
        return OpenedGUI == InventoryGUI;
    }

    public override void _Input(InputEvent e)
    {
        if (dead) return;

        if(OpenedGUI == null)
        {
            if (e is InputEventMouseMotion)
            {
                Vector3 rot = this.GetRotation();

                InputEventMouseMotion emm = (InputEventMouseMotion)e;

                Vector2 rel = emm.GetRelative();

                Vector3 rotd = new Vector3(-rel.y * camRotateSpeed, -rel.x * camRotateSpeed, 0.0f);

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
                            Item i = this.ItemInHand.GetItem();
                            if (i is ItemBlock curBlock)
                            {
                                if (curBlock.Block == b)
                                {
                                    this.ItemInHand.AddToQuantity(1);
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

    public void ReplenishHunger(float v)
    {
        this.CurrentHunger += v;

        if (this.CurrentHunger > MAX_HUNGER)
            this.CurrentHunger = MAX_HUNGER;
    }

    public void ReplenishAir(float v)
    {
        if (Single.IsNaN(v))
            return;

        this.CurrentAir += v;

        if (this.CurrentAir > MAX_AIR)
            this.CurrentAir = MAX_AIR;
    }

    public void ReplenishThirst(float v)
    {
        this.CurrentThirst += v;

        if (this.CurrentThirst > MAX_THIRST)
            this.CurrentThirst = MAX_THIRST;
    }

    public void DepleteHunger(float v)
    {
        this.CurrentHunger -= v;

        if (this.CurrentHunger < 0)
        {
            this.CurrentHunger = 0;
            this.Kill();
        }
    }

    public void DepleteAir(float v)
    {
        this.CurrentAir -= v;

        if (this.CurrentAir < 0)
        {
            this.CurrentAir = 0;
            this.Kill();
        }
    }

    public void DepleteThirst(float v)
    {
        this.CurrentThirst -= v;

        if (this.CurrentThirst < 0)
        {
            this.CurrentThirst = 0;
            this.Kill();
        }
    }

    public void Push(Vector3 v)
    {
        velocity += v;
    }

    public void Kill()
    {
        if (!DEBUG_DEATH_ENABLED) 
        {
            GD.Print("Tried to kill player, but death is disabled");
            return;
        }

        this.dead = true;

        if (OpenedGUI != null)
            this.CloseGUI();
        else
            this.RemoveChild(playerGUI);

        DeadGUI dg = new DeadGUI(this);
        this.AddChild(dg);
    }

    public bool IsDead()
    {
        return dead;
    }

    public void HandleUseItem()
    {
        if (this.ItemInHand == null)
            return;

        Item i = this.ItemInHand.GetItem();

        bool success = false;
        if (i is ItemBlock b)
        {
            success = this.interaction.PlaceBlock(b.Block);
        }
        else if (i is ItemFood f)
        {
            ReplenishHunger(f.ReplenishValue);

            success = true;
        }
        else if (i is ItemDrink d)
        {
            ReplenishThirst(d.ReplenishValue);

            success = true;
        }
        else if (i is ItemPlant p)
        {
            IntVector3? blockPos = this.interaction.GetBlockUnderCursor();
            if (blockPos.HasValue)
                success = plants.Plant(p, blockPos.Value);
        }
        else if (i is ItemBacteriaVial vial)
        {
            game.World.Bacteria.TryGetBacteria(vial.BacteriaType(), out Bacteria bacteria);
            bacteria.AddAmt(vial.Amount);
            success = true;
        }

        if (success)
        {
            this.ItemInHand.SubtractCount(1);
            if (this.ItemInHand.GetCount() == 0)
            {
                this.ItemInHand = null;
            }
            this.InventoryGUI.UpdateHandSlot();
        }
    }

    private bool onFloor;

    //These numbers control how the player's needs change as they move around the world
    
    private static float DEGRED_BALANCE_AIR = 2.0f;
    private static float DEGRED_BALANCE_THIRST = 1.4f;
    private static float DEGRED_BALANCE_HUNGER = 1.8f;

    private static float BASIC_DEGRED = 0.001f;
    private static float MOVE_DEGRED = 0.005f;
    //single frame
    private static float JUMP_DEGRED = 0.05f;
    
    private static float BASE_AIR_REGEN = 0.025f;
    private static float SPRINT_DEGRED_MULT = 4.0f;

    private static float ATMOSPHERE_AIR_REGEN = 0.020f;

    private void ProcessAlive(float delta)
    {
        if (Input.IsActionJustPressed("debug_kill"))
        {
            this.Kill();
        }
        
        Vector3 position = this.GetTranslation();

        if (position.x * position.x + position.z * position.z < WorldGenerator.BASE_RADIUS_SQRD)
        {
            this.ReplenishAir(BASE_AIR_REGEN * delta);
        }

        this.ReplenishAir(ATMOSPHERE_AIR_REGEN * delta * Mathf.Pow(atmosphere.GetGasProgress(Gas.OXYGEN),2));

        //Basic degredation
        this.DepleteAir(BASIC_DEGRED * delta * DEGRED_BALANCE_AIR);
        this.DepleteThirst(BASIC_DEGRED * delta * DEGRED_BALANCE_THIRST);
        this.DepleteHunger(BASIC_DEGRED * delta * DEGRED_BALANCE_HUNGER);

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

        if (OpenedGUI == null)
        {
            if (Input.IsActionJustPressed("jump") && onFloor)
            {
                this.DepleteAir(JUMP_DEGRED * delta * DEGRED_BALANCE_AIR);
                this.DepleteThirst(JUMP_DEGRED * delta * DEGRED_BALANCE_THIRST);
                this.DepleteHunger(JUMP_DEGRED * delta * DEGRED_BALANCE_HUNGER);

                velocity += new Vector3(0.0f, jumpPower, 0.0f);
            }
        }

        velocity += new Vector3(0,-gravity * delta,0);

        Vector3 rot = myCam.GetRotation();

        float sin = (float) Math.Sin(rot.y);
        float cos = (float) Math.Cos(rot.y);

        if (OpenedGUI == null)
        {
            Vector3 movDir = new Vector3();
            if (Input.IsActionPressed("forward"))
            {
                movDir += new Vector3(-sin,0.0f,-cos);
            }
            if (Input.IsActionPressed("backward"))
            {
                movDir += new Vector3(sin,0.0f,cos);
            }
            if (Input.IsActionPressed("left"))
            {
                movDir += new Vector3(-cos,0.0f,sin);
            }
            if (Input.IsActionPressed("right"))
            {
                movDir += new Vector3(cos,0.0f,-sin);
            }

            if (!movDir.Equals(new Vector3()))
            {
                this.DepleteAir(MOVE_DEGRED * delta * DEGRED_BALANCE_AIR);
                this.DepleteThirst(MOVE_DEGRED * delta * DEGRED_BALANCE_THIRST);
                this.DepleteHunger(MOVE_DEGRED * delta * DEGRED_BALANCE_HUNGER);
            }

            movDir = movDir.Normalized();
            if(Input.IsActionPressed("sprint"))
            {
                this.DepleteAir(MOVE_DEGRED * delta * DEGRED_BALANCE_AIR * SPRINT_DEGRED_MULT);
                this.DepleteThirst(MOVE_DEGRED * delta * DEGRED_BALANCE_THIRST * SPRINT_DEGRED_MULT);
                this.DepleteHunger(MOVE_DEGRED * delta * DEGRED_BALANCE_HUNGER * SPRINT_DEGRED_MULT);
                movDir *= 3f;
            }

            velocity += movDir * moveSpeed * delta;
        }

        velocity = new Vector3(velocity.x * xz_inertia, velocity.y * y_intertia, velocity.z * xz_inertia);

        velocity = this.MoveAndSlide(velocity, new Vector3(0.0f, 1.0f, 0.0f));

        // myCam.SetTranslation(this.physicsBody.GetTranslation() + camOffset);

        onFloor = this.IsOnFloor();
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
        if (dead)
        {
            ProcessDead(delta);
        } else
        {
            ProcessAlive(delta);
        }
    }
}
