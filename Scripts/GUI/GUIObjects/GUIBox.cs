using System;
using Godot;

public class GUIBox : GUIObject
{
    public const string GUI_SHEET = "guiSheet";

    public const int REGION_SIZE = 16;

    // TODO: extract 9 element scaling into separate class, use it also for bars
    public GUIBox(Vector2 pos, Vector2 size, Func<bool> shouldShow = null) : 
        base(pos, size, Game.guiResourceLoader.GetResource(GUI_SHEET) as Texture, size / REGION_SIZE, shouldShow)
    {
        Texture tex = Game.guiResourceLoader.GetResource(GUI_SHEET) as Texture;
        for (int x=0; x < 3; ++x)
        {
            for (int y=0; y < 3; ++y)
            {
                Sprite s = (x==1 && y==1) ? sprite : new Sprite();
                s.Texture = tex;
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
                    AddChild(s);
                }
            }
        }
    }
}