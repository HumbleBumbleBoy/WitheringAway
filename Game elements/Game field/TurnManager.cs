using Godot;
using Witheringaway.Game_elements.lib;

public partial class TurnManager : Node
{
    [Export] TextureButton? passTurnButton;
    [Export] Node? enemyNode;

    public StateMachine<TurnManager> StateMachine = null!;
    
    public bool canPlayerPlaceCards;
    public bool canEnemyPlaceCards;
    public bool isCombatTime;

    public override void _Ready()
    {
        StateMachine = new StateMachine<TurnManager>(this);
        StateMachine.ChangeState(new PlayerTurn());
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
