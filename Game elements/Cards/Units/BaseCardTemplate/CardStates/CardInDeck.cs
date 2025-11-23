using Godot;
using Witheringaway.Game_elements.Cards.Units.BaseCardTemplate;
using Witheringaway.Game_elements.lib;

public class CardInDeck : IState<BaseCardTemplate>
{
    
    public IState<BaseCardTemplate>? OnEnter(BaseCardTemplate card, IState<BaseCardTemplate>? previousState)
    {
        GD.Print(card.Name + " entered deck");
        card.SetFlipped(true); 
        
        return null;
    }

    public IState<BaseCardTemplate>? OnExit(BaseCardTemplate card, IState<BaseCardTemplate>? nextState)
    {
        GD.Print(card.Name + " exited deck");
        card.SetFlipped(false);

        return new CardInHand();
    }

}
