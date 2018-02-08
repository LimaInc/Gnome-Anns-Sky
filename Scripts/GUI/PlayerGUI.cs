using System;
using Godot;

public class PlayerGUI : GUI
{
    private static Color AIR_COLOR = new Color(0.0f, 1.0f, 1.0f);
    private static Color THIRST_COLOR = new Color(0.0f, 0.0f, 1.0f);
    private static Color HUNGER_COLOR = new Color(0.0f, 0.7f, 0.2f);

    private static Color ATMOS0_COLOR = new Color(1.0f, 0.0f, 0.0f);
    private static Color ATMOS1_COLOR = new Color(0.0f, 1.0f, 0.0f);
    private static Color ATMOS2_COLOR = new Color(0.0f, 0.0f, 1.0f);

    private Player player;

    private GUIVerticalBar air;
    private GUIVerticalBar thirst;
    private GUIVerticalBar hunger;

    private GUIHorizontalBar atmos0;
    private GUIHorizontalBar atmos1;
    private GUIHorizontalBar atmos2;

    public PlayerGUI(Player p) : base(p)
    {
        this.player = p;
    }

    public override void HandleResize()
    {
        this.RemoveChild(this.atmos0);
        this.RemoveChild(this.atmos1);
        this.RemoveChild(this.atmos2);
        this.RemoveChild(this.air);
        this.RemoveChild(this.thirst);
        this.RemoveChild(this.hunger);

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

        Vector2 baseAtmosOffset = new Vector2(this.GetViewportDimensions().x / 2 - (barHeight - 5.0f) * 1.5f,50.0f);

        Vector2 atmos0Pos = baseAtmosOffset;
        this.atmos0 = new GUIHorizontalBar(atmos0Pos, barHeight, ATMOS0_COLOR);

        Vector2 atmos1Pos = atmos0Pos + new Vector2(barHeight + 80.0f, 0.0f);
        this.atmos1 = new GUIHorizontalBar(atmos1Pos, barHeight, ATMOS1_COLOR);

        Vector2 atmos2Pos = atmos1Pos + new Vector2(barHeight + 80.0f, 0.0f);
        this.atmos2 = new GUIHorizontalBar(atmos2Pos, barHeight, ATMOS2_COLOR);

        this.AddChild(this.atmos0);
        this.AddChild(this.atmos1);
        this.AddChild(this.atmos2);
    }

    public override void _Process(float delta)
    {
        base._Process(delta);

        this.air.SetPercentage((float) this.player.GetCurrentAir() / Player.MAX_AIR);
        this.thirst.SetPercentage((float) this.player.GetCurrentThirst() / Player.MAX_THIRST);
        this.hunger.SetPercentage((float) this.player.GetCurrentHunger() / Player.MAX_HUNGER);

        this.atmos0.SetPercentage(0.7f);
        this.atmos1.SetPercentage(0.2f);
        this.atmos2.SetPercentage(0.5f);
    }
}