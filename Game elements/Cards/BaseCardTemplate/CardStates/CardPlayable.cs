using Godot;
using System;
using Witheringaway.Game_elements.lib;

public class CardPlayable : IState<BaseCardTemplate>
{
    public IState<BaseCardTemplate>? OnEnter(BaseCardTemplate context, IState<BaseCardTemplate>? previousState)
    {
        GD.Print(context.Name + " is playable");
        context.isCardPlayable = true;
        
        return null;
    }

    public IState<BaseCardTemplate>? OnExit(BaseCardTemplate context, IState<BaseCardTemplate>? nextState)
    {
        GD.Print(context.Name + " no longer playable");
        context.isCardPlayable = false;
        
        return null;
    }

}
