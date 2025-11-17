using Godot;
using System;

public partial class CardEnteredField : CardState
{   
    public override void Enter(BaseCardTemplate card, ref CardState? optionalState)
    {
        GD.Print(card + " entered field");
        card.MouseDefaultCursorShape = Control.CursorShape.PointingHand;
    }

    public override void Exit(BaseCardTemplate card, ref CardState? optionalState)
    {
        GD.Print(card + "exited field");  // probably died
        optionalState = new CardDied();
    }
}
