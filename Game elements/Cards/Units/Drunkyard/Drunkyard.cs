using Godot;
using System;
using Witheringaway.Game_elements.Cards.Units.BaseCardTemplate;

public partial class Drunkyard : Witheringaway.Game_elements.Cards.Units.BaseCardTemplate.BaseCardTemplate
{
    // +1ATK every time it deals damage and survives

    public override void OnAttackLanded(BaseCardTemplate? target, bool isPlayer)
    {
        base.OnAttackLanded(target, isPlayer);
        
        if (target == null || !IsInstanceValid(target)) return;
        
        BuffAttackDamage(1);
    }
}
