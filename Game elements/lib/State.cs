namespace Witheringaway.Game_elements.lib;

public interface State<TValue>
{

    public State<TValue>? OnEnter(TValue context, State<TValue>? previousState)
    {
        return null;
    }

    public State<TValue>? OnExit(TValue context, State<TValue>? nextState)
    {
        return null;
    }

    public State<TValue>? OnUpdate(TValue context, double deltaTime)
    {
        return null;
    }

    public State<TValue>? OnFixedUpdate(TValue context, double deltaTime)
    {
        return null;
    }
    
}