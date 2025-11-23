using Godot;
using System;
using Witheringaway.Game_elements.Cards.Units.BaseCardTemplate;
using Witheringaway.Game_elements.lib.manager;

public partial class Informant : Witheringaway.Game_elements.Cards.Units.BaseCardTemplate.BaseCardTemplate
{
    // on hit draw a card

    protected override void OnDamageTaken(BaseCardTemplate? attacker, int damage, bool isPlayer)
    {
        base.OnDamageTaken(attacker, damage, isPlayer);
        
        HandManager.GetHandManager(isPlayer).GetTopCard();
    }
}
