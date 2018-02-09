using System;
using Godot;

public class GUIBox : GUIObject
{
    public static Texture GUI_SHEET = ResourceLoader.Load("res://Images/guiSheet.png") as Texture;

    private Sprite topLeft = new Sprite();
    private Sprite topRight = new Sprite();
    private Sprite bottomLeft = new Sprite();
    private Sprite bottomRight = new Sprite();
    
    private Sprite top = new Sprite();
    private Sprite left = new Sprite();
    private Sprite right = new Sprite();
    private Sprite bottom = new Sprite();

    public GUIBox(Rect2 r) : base(r, GUI_SHEET, r.Size / 16.0f)
    {
        this.sprite.SetRegion(true);
        this.sprite.SetRegionRect(new Rect2(16, 16, 16, 16));

        topLeft.SetTexture(GUI_SHEET);
        topLeft.SetRegion(true);
        topLeft.SetRegionRect(new Rect2(0, 0, 16, 16));
        topLeft.SetPosition(new Vector2(r.Position.x - r.Size.x / 2.0f, r.Position.y - r.Size.y / 2.0f));
        this.AddChild(topLeft);

        topRight.SetTexture(GUI_SHEET);
        topRight.SetRegion(true);
        topRight.SetRegionRect(new Rect2(32, 0, 16, 16));
        topRight.SetPosition(new Vector2(r.Position.x + r.Size.x / 2.0f, r.Position.y - r.Size.y / 2.0f));
        this.AddChild(topRight);

        bottomLeft.SetTexture(GUI_SHEET);
        bottomLeft.SetRegion(true);
        bottomLeft.SetRegionRect(new Rect2(0, 32, 16, 16));
        bottomLeft.SetPosition(new Vector2(r.Position.x - r.Size.x / 2.0f, r.Position.y + r.Size.y / 2.0f));
        this.AddChild(bottomLeft);

        bottomRight.SetTexture(GUI_SHEET);
        bottomRight.SetRegion(true);
        bottomRight.SetRegionRect(new Rect2(32, 32, 16, 16));
        bottomRight.SetPosition(new Vector2(r.Position.x + r.Size.x / 2.0f, r.Position.y + r.Size.y / 2.0f));
        this.AddChild(bottomRight);

        top.SetTexture(GUI_SHEET);
        top.SetRegion(true);
        top.SetRegionRect(new Rect2(16, 0, 16, 16));
        top.SetPosition(new Vector2(r.Position.x, r.Position.y - r.Size.y / 2.0f));
        top.SetScale(new Vector2(r.Size.x / 16.0f,1.0f));
        this.AddChild(top);

        bottom.SetTexture(GUI_SHEET);
        bottom.SetRegion(true);
        bottom.SetRegionRect(new Rect2(16, 32, 16, 16));
        bottom.SetPosition(new Vector2(r.Position.x, r.Position.y + r.Size.y / 2.0f));
        bottom.SetScale(new Vector2(r.Size.x / 16.0f,1.0f));
        this.AddChild(bottom);

        right.SetTexture(GUI_SHEET);
        right.SetRegion(true);
        right.SetRegionRect(new Rect2(32, 16, 16, 16));
        right.SetPosition(new Vector2(r.Position.x + r.Size.x / 2.0f, r.Position.y));
        right.SetScale(new Vector2(1.0f,r.Size.y / 16.0f));
        this.AddChild(right);

        left.SetTexture(GUI_SHEET);
        left.SetRegion(true);
        left.SetRegionRect(new Rect2(0, 16, 16, 16));
        left.SetPosition(new Vector2(r.Position.x - r.Size.x / 2.0f, r.Position.y));
        left.SetScale(new Vector2(1.0f,r.Size.y / 16.0f));
        this.AddChild(left);
    } 
}