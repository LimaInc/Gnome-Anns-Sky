using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Colors
{
    public static readonly Color CYAN = new CMYKColor(1,0,0,0).AsRGB();
    public static readonly Color MAGENTA = new CMYKColor(0,1,0,0).AsRGB();
    public static readonly Color YELLOW = new CMYKColor(0,0,1,0).AsRGB();
    public static readonly Color BLACK = new CMYKColor(0,0,0,1).AsRGB();
    public static readonly Color WHITE = new CMYKColor(1,1,1,0).AsRGB();
    public static readonly Color GRAY = new Color(0.5f, 0.5f, 0.5f);
    public static readonly Color DARK_GRAY = new Color(0.10f, 0.15f, 0.19f);

    public static Color Add(Color a, Color b)
    {
        return new Color(a.r + b.r, a.g + b.g, a.b + b.b);
    }
}