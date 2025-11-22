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
    
    public void ApplyDefenseOnDamage(ref int damage)
    {
        damage -= Defense;
        if (damage < 0)
        {
            damage = 0;
        }
    }
    
}