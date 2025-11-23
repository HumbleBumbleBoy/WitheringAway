using Godot;
using System;
using Witheringaway.Game_elements.Cards.Units.BaseCardTemplate;
using Witheringaway.Game_elements.lib.manager;

public partial class PhoneGuy : BaseCardTemplate
{
    // on played draw 1 card

    public override void OnSelfEnterField(bool isPlayer)
    {
        base.OnSelfEnterField(isPlayer);
        
        HandManager.GetHandManager(isPlayer).GetTopCard();
    }
}
