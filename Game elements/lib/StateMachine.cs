using Godot;

namespace Witheringaway.Game_elements.lib;

public partial class StateMachine<TValue>(TValue context) : Node
{
    private IState<TValue>? currentState;
    
    public IState<TValue>? CurrentState => currentState;
    public TValue GetContext() => context;

    public void ChangeState(IState<TValue>? newState)
    {
        var exitResult = currentState?.OnExit(GetContext(), newState);
        if (exitResult != null)
        {
            newState = exitResult;
        }

        var previousState = currentState;
        currentState = newState;

        var enterResult = currentState?.OnEnter(GetContext(), previousState);
        if (enterResult != null)
        {
            currentState = enterResult;
        }
    }

    public void ExitState()
    {
        var exitResult = currentState?.OnExit(GetContext(), null);
        if (exitResult != null)
        {
            ChangeState(exitResult);
            return;
        }

        currentState = null;
    }

    public override void _Ready()
    {
        var result = CurrentState?.OnUpdate(GetContext(), GetProcessDeltaTime());
        if (result != null)
        {
            ChangeState(result);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        var result = CurrentState?.OnFixedUpdate(GetContext(), delta);
        if (result != null)
        {
            ChangeState(result);
        }
    }
}