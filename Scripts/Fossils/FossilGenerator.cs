using System;
using System.Collections.Generic;
using Godot;

class FossilGenerator
{
    public FossilBlock Block { get; }

    public FossilGenerator(FossilBlock block)
    {
        Block = block;
    }
}
