using Godot;
using System;

public partial class CardPlayable : CardState
{
    public override void Enter(BaseCardTemplate card, ref CardState? optionalState)
    {
        GD.Print(card + " is playable");
        card.isCardPlayable = true;
    }

    public override void Exit(BaseCardTemplate card, ref CardState? optionalState)
    {
        GD.Print(card + " no longer playable");
        card.isCardPlayable = false;
    }
}
