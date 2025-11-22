using System;
using Godot;

namespace Witheringaway.Game_elements.components;

[GlobalClass]
public partial class DefenseComponent : Component
{
    
    [Export] public int Defense { get; private set; }

    public void SetDefense(int amount)
    {
        Defense = amount;
    }
    
    public int AbsorbDamage(int damage)
    {
        damage -= Defense;
        
        return Math.Clamp(damage, 0, int.MaxValue);
    }
    
}