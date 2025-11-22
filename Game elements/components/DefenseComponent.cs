using System;
using Godot;

namespace Witheringaway.Game_elements.components;

[GlobalClass]
public partial class DefenseComponent : Component
{
    
    public delegate void DefenseChangedEventHandler(int oldTimeOnField, int currentTimeOnField);
    
    public event DefenseChangedEventHandler? OnDefenseChanged;
    
    [Export] public int Defense { get; private set; }

    public void SetDefense(int amount)
    {
        var oldDefense = Defense;
        Defense = amount;
        
        OnDefenseChanged?.Invoke(oldDefense, Defense);
    }
    
    public int AbsorbDamage(int damage)
    {
        damage -= Defense;
        
        return Math.Clamp(damage, 0, int.MaxValue);
    }
    
}