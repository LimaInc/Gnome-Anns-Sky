using Godot;
using System;

public class Player : KinematicBody
{
    private float moveSpeed = 30.0f;
    private float jumpPower = 12.0f;
    private float camRotateSpeed = 0.01f;

    private float xz_inertia = 0.85f;
    private float y_intertia = 1;

    private Vector3 velocity = new Vector3();

    private Vector3 camOffset = new Vector3(0.0f, 0.4f, 0.0f);

    private float gravity = 40.0f;

    private CollisionShape collisionShape;
    private Camera myCam;

    private bool inventoryOpen = false;

    private InventoryGUI inventoryGUI;

    private Inventory consumableInventory;
    private Inventory fossilInventory;
    private Inventory blockInventory;

    public static Texture CURSOR = ResourceLoader.Load("res://Images/cursor.png") as Texture;

    public override void _Ready()
    {
        Input.SetCustomMouseCursor(CURSOR);
        Input.SetMouseMode(Input.MouseMode.Captured);

        collisionShape = new CollisionShape();

        // BoxShape b = new BoxShape();
        // b.SetExtents(new Vector3(Chunk.BLOCK_SIZE / 2.0f - 0.05f, Chunk.BLOCK_SIZE - 0.05f,Chunk.BLOCK_SIZE / 2.0f - 0.05f));
        // collisionShape.SetShape(b);

        CapsuleShape c = new CapsuleShape();
        c.SetRadius(Chunk.BLOCK_SIZE / 2.0f - 0.05f);
        c.SetHeight( Chunk.BLOCK_SIZE - 0.05f);
        collisionShape.SetShape(c);
        collisionShape.Rotate(new Vector3(1.0f, 0.0f, 0.0f), (float) Math.PI / 2.0f);

        this.AddChild(collisionShape);
        this.SetTranslation(new Vector3(0.0f,40.0f,0.0f));

        myCam = (Camera) this.GetChild(0);

        myCam.SetTranslation(camOffset);

        consumableInventory = new Inventory(this, Item.Type.CONSUMABLE);
        fossilInventory = new Inventory(this, Item.Type.FOSSIL);
        blockInventory = new Inventory(this, Item.Type.BLOCK);

        blockInventory.AddItem(ItemStorage.block, 20);
        fossilInventory.AddItem(ItemStorage.fossil, 10);
        consumableInventory.AddItem(ItemStorage.chocoloate, 10);
        blockInventory.AddItem(ItemStorage.block, 15);
        blockInventory.AddItem(ItemStorage.block, 34);
    }

    public CollisionShape GetCollisionShape()
    {
        return this.collisionShape;
    }

    public bool isInventoryOpen()
    {
        return inventoryOpen;
    }

    public override void _Input(InputEvent e)
    {
        if (e is InputEventMouseMotion && !inventoryOpen)
        {
            Vector3 rot = this.GetRotation();

            InputEventMouseMotion emm = (InputEventMouseMotion) e;

            Vector2 rel = emm.GetRelative();

            int relX = (int) -rel.y;
            int relY = (int) -rel.x;

            Vector3 rotd = new Vector3(relX * camRotateSpeed, relY * camRotateSpeed, 0.0f);

            Vector3 targetRotation = myCam.GetRotation() + rotd;

            //Clamp x rotation between -180 and 180 degrees
            float xRot = targetRotation.x;
            xRot = Mathf.Clamp(xRot, -Mathf.PI / 2, Mathf.PI / 2);
            targetRotation = new Vector3(xRot, targetRotation.y, targetRotation.z);

            myCam.SetRotation(targetRotation);
        }
    }

    private bool onFloor;

    public override void _Process(float delta)
    {
        if (Input.IsActionJustPressed("inventory"))
        {
            if (!inventoryOpen)
            {
                inventoryGUI = new InventoryGUI(consumableInventory, fossilInventory, blockInventory, this.GetViewport().Size);
                inventoryOpen = true;
                this.AddChild(inventoryGUI);
                Input.SetMouseMode(Input.MouseMode.Visible);
            } else 
            {
                inventoryOpen = false;
                inventoryGUI.HandleClose();
                this.RemoveChild(inventoryGUI);
                Input.SetMouseMode(Input.MouseMode.Captured);
            }
        }

        if (!inventoryOpen)
        {
            if (Input.IsActionJustPressed("jump") && onFloor)
            {
                velocity += new Vector3(0.0f, jumpPower, 0.0f);
            }
        }

        velocity += new Vector3(0,-gravity * delta,0);

        Vector3 rot = myCam.GetRotation();

        float sin = (float) Math.Sin(rot.y);
        float cos = (float) Math.Cos(rot.y);

        if (!inventoryOpen)
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
            movDir = movDir.Normalized();
            if(Input.IsActionPressed("sprint"))
            {
                movDir *= 3f;
            }

            velocity += movDir * moveSpeed * delta;
        }

        velocity = new Vector3(velocity.x * xz_inertia, velocity.y * y_intertia, velocity.z * xz_inertia);

        velocity = this.MoveAndSlide(velocity, new Vector3(0.0f, 1.0f, 0.0f));

        // myCam.SetTranslation(this.physicsBody.GetTranslation() + camOffset);

        onFloor = this.IsOnFloor();
        GD.Print(onFloor);
    }
}
