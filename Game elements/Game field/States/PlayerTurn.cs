using Godot;
using System;

public partial class PlayerTurn : TurnState
{
    public override void Enter(TurnManager turnManager)
    {
        turnManager.canPlayerPlaceCards = true;
        turnManager.EnablePassTurnButton();
    }

    public override void Exit(TurnManager turnManager)
    {
        turnManager.DisalePassTurnButton();
        turnManager.canPlayerPlaceCards = false;
    }
}
