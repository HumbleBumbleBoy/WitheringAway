using Witheringaway.Game_elements.lib;

public class Combat : State<TurnManager>
{
    public State<TurnManager>? OnEnter(TurnManager context, State<TurnManager>? previousState)
    {
        context.isCombatTime = true;
        return null;
    }

    public State<TurnManager>? OnExit(TurnManager context, State<TurnManager>? nextState)
    {
        context.isCombatTime = false;
        return null;
    }
}