using Godot;
using System;

public partial class CostManager : Node
{
    public int Cost;
    public int CurrentCost;

    public void Initialize()
    {
        CurrentCost = Cost;
    }
}
