using System;
using Godot;

public class PlayerGUI : GUI
{
    private static Color AIR_COLOR = new Color(0.0f, 1.0f, 1.0f);
    private static Color THIRST_COLOR = new Color(0.0f, 0.0f, 1.0f);
    private static Color HUNGER_COLOR = new Color(0.0f, 0.7f, 0.2f);

    private Player player;

    private GUIBar air;
    private GUIBar thirst;
    private GUIBar hunger;

    public PlayerGUI(Player p) : base(p)
    {
        this.player = p;
    }

    public override void HandleResize()
    {
        this.RemoveChild(this.air);
        this.RemoveChild(this.thirst);
        this.RemoveChild(this.hunger);

        float barHeight = 200.0f;

        Vector2 baseOffset = new Vector2(50.0f, this.GetViewportDimensions().y - barHeight / 2.0f - 40.0f);

        Vector2 airPos = baseOffset;
        this.air = new GUIBar(airPos, barHeight, AIR_COLOR);

        Vector2 thirstPos = airPos + new Vector2(30.0f + GUIBar.WIDTH, 0.0f);
        this.thirst = new GUIBar(thirstPos, barHeight, THIRST_COLOR);

        Vector2 hungerPos = thirstPos + new Vector2(30.0f + GUIBar.WIDTH, 0.0f);
        this.hunger = new GUIBar(hungerPos, barHeight, HUNGER_COLOR);

        this.AddChild(this.air);
        this.AddChild(this.thirst);
        this.AddChild(this.hunger);
    }

    public override void _Process(float delta)
    {
        base._Process(delta);

        this.air.SetPercentage((float) this.player.GetCurrentAir() / Player.MAX_AIR);
        this.thirst.SetPercentage((float) this.player.GetCurrentThirst() / Player.MAX_THIRST);
        this.hunger.SetPercentage((float) this.player.GetCurrentHunger() / Player.MAX_HUNGER);
    }
}