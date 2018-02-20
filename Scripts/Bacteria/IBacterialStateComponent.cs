using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public interface IBacterialStateComponent
{
    void Update(float delta, ExoWorld w, BacterialState s);
}