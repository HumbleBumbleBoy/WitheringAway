using Godot;
using Witheringaway.Game_elements.lib;

public partial class TurnManager : Node
{
    [Export] TextureButton? passTurnButton;
    [Export] Node? enemyNode;

    public StateMachine<TurnManager> StateMachine { get; }

    public int CurrentRound = 1;
    
    public bool canPlayerPlaceCards;
    public bool canEnemyPlaceCards;
    public bool isCombatTime;

    public TurnManager()
    {
        StateMachine = this.CreateStateMachine(this);
    }
    
    public override void _Ready()
    {
        StateMachine.ChangeState(new PlayerTurn());
    }

    public void EnablePassTurnButton()
    {
        if (passTurnButton is null) return;
        
        passTurnButton.Disabled = false;
        passTurnButton.MouseDefaultCursorShape = Control.CursorShape.PointingHand;
    }

    public void DisalePassTurnButton()
    {
        if (passTurnButton is null) return;
        
        passTurnButton.Disabled = true;
        passTurnButton.MouseDefaultCursorShape = Control.CursorShape.Arrow;
    }
}
