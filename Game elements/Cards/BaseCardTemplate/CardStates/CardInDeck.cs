using Godot;
using System;

public partial class CardInDeck : CardState
{
    public override void Enter(BaseCardTemplate card, ref CardState? optionalState)
    {
        GD.Print(card.Name + " entered deck");
        card.isFlipped = true; 
    }

    public override void Exit(BaseCardTemplate card, ref CardState? optionalState)
    {
        GD.Print(card.Name + " exited deck");
        card.isFlipped = false;
        optionalState = new CardInHand();
    }
}
