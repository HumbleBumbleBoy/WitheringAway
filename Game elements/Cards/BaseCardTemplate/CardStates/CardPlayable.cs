using Godot;
using System;

public partial class CardPlayable : CardState
{
    public override void Enter(BaseCardTemplate card, ref CardState? optionalState)
    {
        GD.Print(card.Name + " is playable");
        card.isCardPlayable = true;
    }

    public override void Exit(BaseCardTemplate card, ref CardState? optionalState)
    {
        GD.Print(card.Name + " no longer playable");
        card.isCardPlayable = false;
    }
}
