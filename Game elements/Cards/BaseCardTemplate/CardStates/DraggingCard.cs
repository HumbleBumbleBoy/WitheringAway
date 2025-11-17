using Godot;
using System;

public partial class DraggingCard : CardState
{
    public override void Enter(BaseCardTemplate card, ref CardState? optionalState)
    {
        GD.Print("dragging " + card);
    }

    public override void Exit(BaseCardTemplate card, ref CardState? optionalState)
    {
        GD.Print("stopped dragging " + card);
        // if valid position, exit to state "cardEnteredField", we remove the card and do the logic there
        // if invalid position, go back to state "cardInHand"
    }
}
