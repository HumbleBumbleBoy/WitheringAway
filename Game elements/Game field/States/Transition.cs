using Witheringaway.Game_elements.lib;

public class Transition : IState<TurnManager>
{
    public IState<TurnManager>? OnEnter(TurnManager context, IState<TurnManager>? previousState)
    {

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