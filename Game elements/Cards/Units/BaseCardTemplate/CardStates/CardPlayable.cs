using Godot;
using Witheringaway.Game_elements.Cards.BaseCardTemplate;
using Witheringaway.Game_elements.lib;

public class CardPlayable : IState<BaseCardTemplate>
{
    public IState<BaseCardTemplate>? OnEnter(BaseCardTemplate context, IState<BaseCardTemplate>? previousState)
    {
        GD.Print(context.Name + " is playable");
        context.IsCardPlayable = true;
        
        return null;
    }

    public IState<BaseCardTemplate>? OnExit(BaseCardTemplate context, IState<BaseCardTemplate>? nextState)
    {
        GD.Print(context.Name + " no longer playable");
        context.IsCardPlayable = false;
        
        return null;
    }

}
