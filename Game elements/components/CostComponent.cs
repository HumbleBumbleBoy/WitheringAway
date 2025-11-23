using System;
using Godot;

namespace Witheringaway.Game_elements.components;

[GlobalClass]
public partial class CostComponent : Component
{
    
    public delegate void CostChangedEventHandler(int oldCost, int currentCost);
    
    public event CostChangedEventHandler? OnCostChanged;
    
    [Export] public int Cost { get; private set; }

    public void SetCost(int amount)
    {
        var oldCost = Cost;
        Cost = amount;
        
        OnCostChanged?.Invoke(oldCost, Cost);
    }
    
}