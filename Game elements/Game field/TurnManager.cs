using System.Reflection;
using Godot;

public partial class TurnManager : Node
{
    [Export] TextureButton? passTurnButton;
    [Export] Node? enemyNode;

    protected TurnState? currentState;
    public bool canPlayerPlaceCards;
    public bool canEnemyPlaceCards;
    public bool isCombatTime;

    public override void _Ready()
    {
        ChangeState(new PlayerTurn());
    }

    public void ChangeState(TurnState newState)
    {
        GD.Print($"Changing state from {currentState?.GetType().Name} to {newState.GetType().Name}");
        
        currentState?.Exit(this);
        currentState = newState;
        currentState?.Enter(this);
        
        GD.Print($"Current state is now: {currentState?.GetType().Name}");
    }

    public void ExitCurrentState()
    {
        currentState?.Exit(this);
        currentState = null;
    }

    public void EnablePassTurnButton()
    {
        passTurnButton.Disabled = false;
        passTurnButton.MouseDefaultCursorShape = Control.CursorShape.PointingHand;
    }

    public void DisalePassTurnButton()
    {
        passTurnButton.Disabled = true;
        passTurnButton.MouseDefaultCursorShape = Control.CursorShape.Arrow;
    }
    // if canPlayerPlaceCards then throw out a signal or osme shit and add a vareiable in base player card 
    // that allows to place cards when thats true and not allow it when its false
    // same for other states yk
}
