using System;
using Godot;
using System.Collections.Generic;

public class PlayerGUI : GUI
{
    private static Texture AIR_ICON_TEX = ResourceLoader.Load(Game.GUI_TEXTURE_PATH + "airIcon.png") as Texture;
    public static Texture CROSSHAIR_TEX = ResourceLoader.Load(Game.GUI_TEXTURE_PATH + "crosshairWhite.png") as Texture;

    private readonly static IDictionary<Gas, Color> gasBarColors = new Dictionary<Gas, Color>
    {
        [Gas.OXYGEN] = Colors.MAGENTA,
        [Gas.NITROGEN] = Colors.CYAN,
        [Gas.CARBON_DIOXIDE] = Colors.YELLOW
    };
    private readonly static IDictionary<Gas, float> gasBarOffsetIndices = new Dictionary<Gas, float>
    {
        [Gas.OXYGEN] = -1,
        [Gas.NITROGEN] = 0,
        [Gas.CARBON_DIOXIDE] = 1
    };
    private readonly static IDictionary<Player.Stats, Color> statBarColors = new Dictionary<Player.Stats, Color>
    {
        [Player.Stats.AIR] = gasBarColors[Gas.OXYGEN],
        [Player.Stats.WATER] = new Color(0.0f, 0.0f, 1.0f),
        [Player.Stats.FOOD] = new Color(0.0f, 0.7f, 0.2f)
    };
    private readonly static IDictionary<Player.Stats, float> statBarOffsetIndices = new Dictionary<Player.Stats, float>
    {
        [Player.Stats.AIR] = 0,
        [Player.Stats.WATER] = 1,
        [Player.Stats.FOOD] = 2
    };
    private readonly static IDictionary<Player.Stats, Sprite> statBarSprites = new Dictionary<Player.Stats, Sprite>
    {
        [Player.Stats.WATER] = ItemStorage.items[ItemID.WATER].GenerateGUISprite(),
        [Player.Stats.FOOD] = ItemStorage.items[ItemID.CAKE].GenerateGUISprite(),
        [Player.Stats.AIR] = new Sprite
        {
            Texture = AIR_ICON_TEX,
        }
    };

    private const float BAR_LENGTH = 200;
    private const float BAR_SEPARATION = 30;
    private const float BAR_EDGE_OFFSET = 20;
    private readonly static Vector2 ICON_SCALE = new Vector2(2, 2);
    private readonly static Vector2 ICON_OFFSET = new Vector2(30, -20);

    private const float ATM_BAR_LENGTH = 280;
    private const float ATM_BAR_EDGE_OFFSET = 25;
    private const float ATM_BAR_SPACING = 50;

    private Player player;

    private readonly static IDictionary<Gas, GUIHorizontalBar> gasBars = new Dictionary<Gas, GUIHorizontalBar>();
    private readonly static IDictionary<Player.Stats, GUIVerticalBar> statBars = new Dictionary<Player.Stats, GUIVerticalBar>();

    private Label2D inHandLabel;

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
        atm = GetNode(Game.ATMOSPHERE_PATH) as Atmosphere;
    }

    public PlayerGUI(Player p) : base(p)
    {
        player = p;
        Initialize();
        BackgroundMode = false;
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

        foreach (KeyValuePair<Player.Stats,Color> kvPair in statBarColors)
        {
            Player.Stats stat = kvPair.Key;
            statBars[stat] = new GUIVerticalBar(empty, BAR_LENGTH, kvPair.Value);
            AddChild(statBars[stat]);
            statBarSprites[stat].Scale = ICON_SCALE;
            statBarSprites[stat].Position = ICON_OFFSET;
            statBars[stat].AddChild(statBarSprites[stat]);
        }
        foreach (KeyValuePair<Gas, Color> kvPair in gasBarColors)
        {
            Gas g = kvPair.Key;
            gasBars[g] = new GUIHorizontalBar(empty, ATM_BAR_LENGTH, kvPair.Value);
            AddChild(gasBars[g]);
        }

        inHandLabel = new Label2D();
        AddChild(inHandLabel);

        crosshair = new Sprite()
        {
            Texture = CROSSHAIR_TEX
        };
        AddChild(crosshair);
    }

    public override void HandleResize()
    {
        Vector2 baseStatOffset = new Vector2(BAR_EDGE_OFFSET, GetViewportDimensions().y - BAR_LENGTH - BAR_EDGE_OFFSET);
        Vector2 statOffsetDiff = new Vector2(BAR_SEPARATION + GUIVerticalBar.WIDTH, 0);
        foreach (KeyValuePair<Player.Stats,GUIVerticalBar> kvPair in statBars)
        {
            kvPair.Value.Position = baseStatOffset + statOffsetDiff * statBarOffsetIndices[kvPair.Key];
        }

        Vector2 baseAtmosOffset = new Vector2(GetViewportDimensions().x / 2 + ATM_BAR_LENGTH / 2, ATM_BAR_EDGE_OFFSET);
        Vector2 amtosOffsetDiff = new Vector2(ATM_BAR_LENGTH + ATM_BAR_SPACING, 0);
        foreach (KeyValuePair<Gas, GUIHorizontalBar> kvPair in gasBars)
        {
            kvPair.Value.Position = baseAtmosOffset + amtosOffsetDiff * gasBarOffsetIndices[kvPair.Key];
        }
        
        inHandLabel.Position = new Vector2(GetViewportDimensions().x / 2 - 80, GetViewportDimensions().y - 15);
        
        crosshair.Position = GetViewportDimensions() / 2;
    }

    public override void _Process(float delta)
    {
        base._Process(delta);

        foreach (KeyValuePair<Player.Stats, GUIVerticalBar> kvPair in statBars)
        {
            kvPair.Value.Percentage = player[kvPair.Key];
        }
        foreach (KeyValuePair<Gas, GUIHorizontalBar> kvPair in gasBars)
        {
            kvPair.Value.Percentage = atm.GetGasProgress(kvPair.Key);
        }

        if (!BackgroundMode && player.ItemInHand != null)
        {
            inHandLabel.Text = "Currently in hand: " + player.ItemInHand.Item.Name + ",    " +
                "Quantity : " + player.ItemInHand.Count;
        }
        else 
        {
            inHandLabel.Text = "";
        }
    }
}
