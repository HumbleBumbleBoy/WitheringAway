using Witheringaway.Game_elements.lib;

public class PlayerTurn : IState<TurnManager>
{
    public IState<TurnManager>? OnEnter(TurnManager context, IState<TurnManager>? previousState)
    {
        context.canPlayerPlaceCards = true;
        context.EnablePassTurnButton();
        
        
        return null;
    }

    public IState<TurnManager>? OnExit(TurnManager context, IState<TurnManager>? nextState)
    {
        context.DisalePassTurnButton();
        context.canPlayerPlaceCards = false;
        return null;
    }

}
