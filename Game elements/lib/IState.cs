namespace Witheringaway.Game_elements.lib;

public interface IState<TValue>
{

    public IState<TValue>? OnEnter(TValue context, IState<TValue>? previousState)
    {
        return null;
    }

    public IState<TValue>? OnExit(TValue context, IState<TValue>? nextState)
    {
        return null;
    }

    public IState<TValue>? OnUpdate(TValue context, double deltaTime)
    {
        return null;
    }

    public IState<TValue>? OnFixedUpdate(TValue context, double deltaTime)
    {
        return null;
    }
    
}