using Godot;
using System;
using Witheringaway.Game_elements.lib;

public class CardInDeck : IState<BaseCardTemplate>
{
    
    public IState<BaseCardTemplate>? OnEnter(BaseCardTemplate card, IState<BaseCardTemplate>? previousState)
    {
        GD.Print(card.Name + " entered deck");
        card.isFlipped = true; 
        
        return null;
    }

    public IState<BaseCardTemplate>? OnExit(BaseCardTemplate card, IState<BaseCardTemplate>? nextState)
    {
        GD.Print(card.Name + " exited deck");
        card.isFlipped = false;

        return new CardInHand();
    }

}
