using System;
using Godot;

public class GUIBox : GUIObject
{
    public static Texture GUI_SHEET = ResourceLoader.Load(Game.GUI_TEXTURE_PATH + "guiSheet.png") as Texture;

    public const int REGION_SIZE = 16;

    public GUIBox(Vector2 pos, Vector2 size) : base(pos, size, GUI_SHEET, size / 16)
    {
        for (int x=0; x < 3; ++x)
        {
            for (int y=0; y < 3; ++y)
            {
                Sprite s = (x==1 && y==1) ? this.sprite : new Sprite();
                s.Texture = GUI_SHEET;
                s.RegionEnabled = true;
                s.RegionRect = new Rect2(x * REGION_SIZE, y * REGION_SIZE, REGION_SIZE, REGION_SIZE);
                if (!(x == 1 && y == 1))
                {
                    s.SetPosition(size / 2 * new Vector2(x - 1, y - 1));
                    if (x == 1 && y != 1)
                    {
                        s.SetScale(new Vector2(size.x / REGION_SIZE, 1));
                    }
                    else if (y==1 && x != 1)
                    {
                        s.SetScale(new Vector2(1, size.y / REGION_SIZE));
                    }
                    this.AddChild(s);
                }
            }
        }
    }
}