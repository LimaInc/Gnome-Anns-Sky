using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Bacteria
{
    public int Amount { get; set; }

    public Bacteria()
    {
        this.Amount = 0;
    }

    public Bacteria(int amt)
    {
        this.Amount = amt;
    }
}