using System;
using Godot;

public class PlayerGUI : GUI
{
    private static Texture AIR_ICON_TEX = ResourceLoader.Load(Game.GUI_TEXTURE_PATH + "airIcon.png") as Texture;
    
    public static Texture CROSSHAIR_TEX = ResourceLoader.Load(Game.GUI_TEXTURE_PATH + "crosshairWhite.png") as Texture;

    private readonly static Color OXYGEN_COLOR = Colors.MAGENTA;
    private readonly static Color NITROGEN_COLOR = Colors.CYAN;
    private readonly static Color CARBON_DIOXIDE_COLOR = Colors.YELLOW;

    private readonly static Color AIR_COLOR = OXYGEN_COLOR;
    private readonly static Color THIRST_COLOR = new Color(0.0f, 0.0f, 1.0f);
    private readonly static Color HUNGER_COLOR = new Color(0.0f, 0.7f, 0.2f);

    private const float BAR_LENGTH = 200;
    private const float BAR_SEPARATION = 30;
    private const float BAR_EDGE_OFFSET = 20;
    private readonly static Vector2 ICON_SCALE = new Vector2(2, 2);
    private readonly static Vector2 ICON_OFFSET = new Vector2(30, -20);

    private const float ATM_BAR_LENGTH = 280;
    private const float ATM_BAR_EDGE_OFFSET = 25;
    private const float ATM_BAR_SPACING = 50;

    private Player player;

    private GUIVerticalBar air;
    private GUIVerticalBar thirst;
    private GUIVerticalBar hunger;

    private Sprite airIcon;
    private Sprite thirstIcon;
    private Sprite hungerIcon;

    private Label2D inHandLabel;

    private GUIHorizontalBar atmosOxygen;
    private GUIHorizontalBar atmosNitrogen;
    private GUIHorizontalBar atmosCarbonDioxide;

    private Sprite crosshair;

    private Atmosphere atm;

    private bool backgroundMode = false;

    public bool BackgroundMode
    {
        get { return backgroundMode; }
        set
        {
            if (backgroundMode && !value)
            {
                ChangeToForegroundMode();
            }
            else if (!backgroundMode && value)
            {
                ChangeToBackgroundMode();
            }
            backgroundMode = value;
        }
    }

    public override void _Ready()
    {
        this.atm = GetNode(Game.ATMOSPHERE_PATH) as Atmosphere;
    }

    public PlayerGUI(Player p) : base(p)
    {
        this.player = p;
        Initialize();
        this.BackgroundMode = false;
    }

    private void ChangeToBackgroundMode()
    {
        crosshair.Hide();
    }

    private void ChangeToForegroundMode()
    {
        crosshair.Show();
        Input.SetMouseMode(Input.MouseMode.Captured);
    }

    public void Initialize()
    {
        Vector2 empty = new Vector2();

        this.air = new GUIVerticalBar(empty, BAR_LENGTH, AIR_COLOR);
        this.thirst = new GUIVerticalBar(empty, BAR_LENGTH, THIRST_COLOR);
        this.hunger = new GUIVerticalBar(empty, BAR_LENGTH, HUNGER_COLOR);

        hungerIcon = ItemStorage.cake.GenerateGUISprite();
        hungerIcon.SetScale(ICON_SCALE);
        thirstIcon = ItemStorage.water.GenerateGUISprite();
        thirstIcon.SetScale(ICON_SCALE);
        airIcon = new Sprite();
        airIcon.SetTexture(AIR_ICON_TEX);
        airIcon.SetScale(ICON_SCALE);
        
        hungerIcon.Position = ICON_OFFSET;
        thirstIcon.Position = ICON_OFFSET;
        airIcon.Position = ICON_OFFSET;

        this.AddChild(this.air);
        this.AddChild(this.thirst);
        this.AddChild(this.hunger);

        this.air.AddChild(airIcon);
        this.thirst.AddChild(thirstIcon);
        this.hunger.AddChild(hungerIcon);

        this.atmosNitrogen = new GUIHorizontalBar(empty, ATM_BAR_LENGTH, NITROGEN_COLOR);
        this.atmosOxygen = new GUIHorizontalBar(empty, ATM_BAR_LENGTH, OXYGEN_COLOR);
        this.atmosCarbonDioxide = new GUIHorizontalBar(empty, ATM_BAR_LENGTH, CARBON_DIOXIDE_COLOR);

        this.AddChild(this.atmosOxygen);
        this.AddChild(this.atmosNitrogen);
        this.AddChild(this.atmosCarbonDioxide);

        inHandLabel = new Label2D();
        this.AddChild(inHandLabel);

        crosshair = new Sprite();
        crosshair.SetTexture(CROSSHAIR_TEX);
        this.AddChild(crosshair);
    }

    public override void HandleResize()
    {
        Vector2 baseStatusOffset = new Vector2(BAR_EDGE_OFFSET, this.GetViewportDimensions().y - BAR_LENGTH - BAR_EDGE_OFFSET);

        Vector2 airPos = baseStatusOffset;
        Vector2 thirstPos = airPos + new Vector2(BAR_SEPARATION + GUIHorizontalBar.WIDTH, 0);
        Vector2 hungerPos = thirstPos + new Vector2(BAR_SEPARATION + GUIHorizontalBar.WIDTH, 0);

        this.air.Position = airPos;
        this.thirst.Position = thirstPos;
        this.hunger.Position = hungerPos;

        Vector2 baseAtmosOffset = new Vector2(this.GetViewportDimensions().x / 2 + ATM_BAR_LENGTH / 2, ATM_BAR_EDGE_OFFSET);

        Vector2 atmosMiddlePos = baseAtmosOffset;
        Vector2 atmosLeftPos = atmosMiddlePos - new Vector2(ATM_BAR_LENGTH + ATM_BAR_SPACING, 0);
        Vector2 atmosRightPos = atmosMiddlePos + new Vector2(ATM_BAR_LENGTH + ATM_BAR_SPACING, 0);
        
        this.atmosNitrogen.Position = atmosLeftPos;
        this.atmosOxygen.Position = atmosMiddlePos;
        this.atmosCarbonDioxide.Position = atmosRightPos;
        
        inHandLabel.Position = new Vector2(this.GetViewportDimensions().x / 2 - 80, this.GetViewportDimensions().y - 15);
        
        crosshair.Position = this.GetViewportDimensions() / 2;
    }

    public override void _Process(float delta)
    {
        base._Process(delta);

        this.air.SetPercentage((float) this.player.CurrentAir / Player.MAX_AIR);
        this.thirst.SetPercentage((float) this.player.CurrentThirst / Player.MAX_THIRST);
        this.hunger.SetPercentage((float) this.player.CurrentHunger / Player.MAX_HUNGER);

        if (this.atm != null)
        {
            this.atmosOxygen.SetPercentage(this.atm.GetGasProgress(Gas.OXYGEN));
            this.atmosNitrogen.SetPercentage(this.atm.GetGasProgress(Gas.NITROGEN));
            this.atmosCarbonDioxide.SetPercentage(this.atm.GetGasProgress(Gas.CARBON_DIOXIDE));
        }
        else
        {
            this.atmosOxygen.SetPercentage(0.0f);
            this.atmosNitrogen.SetPercentage(0.0f);
            this.atmosCarbonDioxide.SetPercentage(0.0f);
        }

        if (!BackgroundMode && player.ItemInHand != null)
        {
            inHandLabel.Text = "Currently in hand: " + player.ItemInHand.GetItem().GetName() + ",    " +
                "Quantity : " + player.ItemInHand.GetCount();
        }
        else 
        {
            inHandLabel.Text = "";
        }
    }
}
