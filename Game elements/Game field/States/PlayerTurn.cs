using Witheringaway.Game_elements.lib;

public class PlayerTurn : State<TurnManager>
{
    public State<TurnManager>? OnEnter(TurnManager context, State<TurnManager>? previousState)
    {
        context.canPlayerPlaceCards = true;
        context.EnablePassTurnButton();
        return null;
    }

    public State<TurnManager>? OnExit(TurnManager context, State<TurnManager>? nextState)
    {
        context.DisalePassTurnButton();
        context.canPlayerPlaceCards = false;
        return null;
    }

}
