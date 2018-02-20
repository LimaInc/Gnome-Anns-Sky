using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

class CMYKColor
{
    public float Cyan { get; private set; }
    public float Magenta { get; private set; }
    public float Yellow { get; private set; }
    public float Black { get; private set; }

    public CMYKColor(float c, float m, float y, float k)
    {
        this.Cyan = c;
        this.Magenta = m;
        this.Yellow = y;
        this.Black = k;
    }

    public Color AsRGB()
    {
        float r = (1 - Cyan) * (1 - Black);
        float g = (1 - Magenta) * (1 - Black);
        float b = (1 - Yellow) * (1 - Black);
        return new Color(r, g, b);
    }
}
