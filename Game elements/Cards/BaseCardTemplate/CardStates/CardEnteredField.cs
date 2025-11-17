using Godot;
using System;

public partial class CardEnteredField : CardState
{   
    public override void Enter(BaseCardTemplate card, ref CardState? optionalState)
    {
        GD.Print(card + " entered field");
    }

    public override void Exit(BaseCardTemplate card, ref CardState? optionalState)
    {
        GD.Print(card + "exited field");  // probably died
        optionalState = new CardDied();
    }
}
