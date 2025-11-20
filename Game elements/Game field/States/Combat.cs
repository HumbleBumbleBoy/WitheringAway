using Godot;
using System;

public partial class Combat : TurnState
{
    public override void Enter(TurnManager turnManager)
    {
        turnManager.isCombatTime = true;
    }

    public override void Exit(TurnManager turnManager)
    {
        turnManager.isCombatTime = false;
    }
}
