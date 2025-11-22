using Godot;

namespace Witheringaway.Game_elements.lib;

public static class StateMachineExtensions
{
    
    public static StateMachine<T> CreateStateMachine<T>(this Node node, T context)
    {
        var stateMachine = new StateMachine<T>(context);
        node.AddChild(stateMachine);
        return stateMachine;
    }
    
}