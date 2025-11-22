using Witheringaway.Game_elements.lib;

public class Combat : IState<TurnManager>
{
    public IState<TurnManager>? OnEnter(TurnManager context, IState<TurnManager>? previousState)
    {
        context.isCombatTime = true;
        return null;
    }

    public IState<TurnManager>? OnExit(TurnManager context, IState<TurnManager>? nextState)
    {
        context.isCombatTime = false;
        return null;
    }
}