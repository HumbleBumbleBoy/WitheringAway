using Godot;
using Witheringaway.Game_elements.lib;
using Witheringaway.Game_elements.lib.manager;

public class Transition : IState<TurnManager>
{
    public IState<TurnManager>? OnEnter(TurnManager context, IState<TurnManager>? previousState)
    {
        var enemyHand = context.GetTree().GetFirstNodeInGroup("EnemyHandManager") as HandManager;
        if (!enemyHand?.HasMoreCards() ?? true)
        {
            GD.Print("Enemy has no more cards. Player wins!");
            return null;
        }
        enemyHand.GetTopCard();
        
        var playerHand = context.GetTree().GetFirstNodeInGroup("PlayerHandManager") as HandManager;
        if (!playerHand?.HasMoreCards() ?? true)
        {
            GD.Print("Player has no more cards. Enemy wins!");
            return null;
        }
        playerHand.GetTopCard();
        
        context.GetTree().CreateTimer(5).Timeout += () =>
        {
            if (context.StateMachine.CurrentState == this)
            {
                context.StateMachine.ChangeState(new PlayerTurn());
            }
        };
        
        return null;
    }
}