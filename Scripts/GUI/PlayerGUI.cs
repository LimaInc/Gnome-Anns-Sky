using System;
using Godot;

public class PlayerGUI : GUI
{
    private Player player;

    private GUIBar air;
    private GUIBar thirst;
    private GUIBar hunger;

    private ColorRect airPercRect;
    private ColorRect thirstPercRect;
    private ColorRect hungerPercRect;

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
        this.air = new GUIBar(airPos, barHeight);

        Vector2 thirstPos = airPos + new Vector2(30.0f + GUIBar.WIDTH, 0.0f);
        this.thirst = new GUIBar(thirstPos, barHeight);

        Vector2 hungerPos = thirstPos + new Vector2(30.0f + GUIBar.WIDTH, 0.0f);
        this.hunger = new GUIBar(hungerPos, barHeight);

        this.AddChild(this.air);
        this.AddChild(this.thirst);
        this.AddChild(this.hunger);
    }
}