using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public abstract class GUIComplexObject : Node2D
{
    public abstract void HandleResize();
}