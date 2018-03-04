using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public abstract class GUIComplexObject : GUIElement
{
    public GUIComplexObject(Func<bool> shouldShow = null) : base(shouldShow) { }

    public abstract void HandleResize();
}