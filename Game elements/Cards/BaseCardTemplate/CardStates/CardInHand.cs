using Godot;
using System;

public partial class CardInHand : CardState
{
    public override void Enter(BaseCardTemplate card, ref CardState? optionalState)
    {
        GD.Print(card.Name + " entered hand");
        card.MouseDefaultCursorShape = Control.CursorShape.PointingHand;
    }

    public override void Exit(BaseCardTemplate card, ref CardState? optionalState)
    {
        GD.Print(card.Name + " exited hand");
        card.MouseDefaultCursorShape = Control.CursorShape.Arrow;
    }
}
