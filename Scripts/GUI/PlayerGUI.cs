using System;
using Godot;

public class PlayerGUI : GUI
{
    private static Texture AIR_ICON_TEX = ResourceLoader.Load("res://Images/airIcon.png") as Texture;
    
    public static Texture CROSSHAIR_TEX = ResourceLoader.Load("res://Images/crosshairWhite.png") as Texture;

    private readonly static Color OXYGEN_COLOR = Colors.MAGENTA;
    private readonly static Color NITROGEN_COLOR = Colors.CYAN;
    private readonly static Color CARBON_DIOXIDE_COLOR = Colors.YELLOW;

    private readonly static Color AIR_COLOR = OXYGEN_COLOR;
    private readonly static Color THIRST_COLOR = new Color(0.0f, 0.0f, 1.0f);
    private readonly static Color HUNGER_COLOR = new Color(0.0f, 0.7f, 0.2f);

    private Player player;

    private GUIVerticalBar air;
    private GUIVerticalBar thirst;
    private GUIVerticalBar hunger;

    private Sprite airIcon;
    private Sprite thirstIcon;
    private Sprite hungerIcon;

    private Label inHandLabel;

    private GUIHorizontalBar atmosOxygen;
    private GUIHorizontalBar atmosNitrogen;
    private GUIHorizontalBar atmosCarbonDioxide;

    private Sprite crosshair;

    private Atmosphere atm;

    public PlayerGUI(Player p) : base(p)
    {
        this.player = p;
    }

    public override void _Ready()
    {
        this.atm = (GetNode(Game.GAME_PATH) as Game).World.Atmosphere;
    }

    bool first = true;

    public override void HandleResize()
    {
        if (!first)
        {
            this.RemoveChild(this.atmosOxygen);
            this.RemoveChild(this.atmosNitrogen);
            this.RemoveChild(this.atmosCarbonDioxide);

            this.RemoveChild(this.air);
            this.RemoveChild(this.thirst);
            this.RemoveChild(this.hunger);

            this.RemoveChild(this.airIcon);
            this.RemoveChild(this.hungerIcon);
            this.RemoveChild(this.thirstIcon);

            this.RemoveChild(this.inHandLabel);
        }

        float barHeight = 200.0f;

        Vector2 baseStatusOffset = new Vector2(50.0f, this.GetViewportDimensions().y - barHeight / 2.0f - 40.0f);

        Vector2 airPos = baseStatusOffset;
        this.air = new GUIVerticalBar(airPos, barHeight, AIR_COLOR);

        Vector2 thirstPos = airPos + new Vector2(30.0f + GUIHorizontalBar.WIDTH, 0.0f);
        this.thirst = new GUIVerticalBar(thirstPos, barHeight, THIRST_COLOR);

        Vector2 hungerPos = thirstPos + new Vector2(30.0f + GUIHorizontalBar.WIDTH, 0.0f);
        this.hunger = new GUIVerticalBar(hungerPos, barHeight, HUNGER_COLOR);

        this.AddChild(this.air);
        this.AddChild(this.thirst);
        this.AddChild(this.hunger);

        hungerIcon = ItemStorage.cake.GenerateGUISprite();
        hungerIcon.SetPosition(hungerPos - new Vector2(0.0f, 150.0f));
        hungerIcon.SetScale(new Vector2(2.0f, 2.0f));
        this.AddChild(hungerIcon);

        thirstIcon = ItemStorage.water.GenerateGUISprite();
        thirstIcon.SetPosition(thirstPos - new Vector2(0.0f, 150.0f));
        thirstIcon.SetScale(new Vector2(2.0f, 2.0f));
        this.AddChild(thirstIcon);

        airIcon = new Sprite();
        airIcon.SetTexture(AIR_ICON_TEX);
        airIcon.SetPosition(airPos - new Vector2(0.0f, 150.0f));
        airIcon.SetScale(new Vector2(2.0f, 2.0f));
        this.AddChild(airIcon);

        Vector2 baseAtmosOffset = new Vector2(this.GetViewportDimensions().x / 2 - (barHeight - 5.0f) * 1.5f,50.0f);

        Vector2 atmos0Pos = baseAtmosOffset;
        this.atmosOxygen = new GUIHorizontalBar(atmos0Pos, barHeight, OXYGEN_COLOR);

        Vector2 atmos1Pos = atmos0Pos + new Vector2(barHeight + 80.0f, 0.0f);
        this.atmosNitrogen = new GUIHorizontalBar(atmos1Pos, barHeight, NITROGEN_COLOR);

        Vector2 atmos2Pos = atmos1Pos + new Vector2(barHeight + 80.0f, 0.0f);
        this.atmosCarbonDioxide = new GUIHorizontalBar(atmos2Pos, barHeight, CARBON_DIOXIDE_COLOR);

        this.AddChild(this.atmosOxygen);
        this.AddChild(this.atmosNitrogen);
        this.AddChild(this.atmosCarbonDioxide);

        inHandLabel = new Label();

        inHandLabel.SetPosition(new Vector2(this.GetViewportDimensions().x / 2.0f - 80.0f, this.GetViewportDimensions().y - 15.0f));

        this.AddChild(inHandLabel);
        crosshair = new Sprite();
        crosshair.SetTexture(CROSSHAIR_TEX);
        crosshair.SetPosition(this.GetViewportDimensions() / 2.0f);
        this.AddChild(crosshair);

        first = false;
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

        if (player.ItemInHand != null)
        {
            inHandLabel.SetText("Currently in hand: " + player.ItemInHand.GetItem().GetName() + ",    Quantity : " + player.ItemInHand.GetCount());
        }
        else 
        {
            inHandLabel.SetText("");
        }
    }
}
