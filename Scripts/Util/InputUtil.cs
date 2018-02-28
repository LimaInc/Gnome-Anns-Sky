using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public static class InputUtil
{
    public const int BUTTON_LEFT = 1;
    public const int BUTTON_RIGHT = 2;

    public static bool IsRighPress(InputEventMouseButton iemb)
    {
        return iemb.ButtonIndex == BUTTON_RIGHT && iemb.Pressed;
    }

    public static bool IsLeftPress(InputEventMouseButton iemb)
    {
        return iemb.ButtonIndex == BUTTON_LEFT && iemb.Pressed;
    }
}
