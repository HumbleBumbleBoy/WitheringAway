using Godot;
using System.Collections.Generic;
using Witheringaway.Game_elements.components;
using Witheringaway.Game_elements.lib.manager;

public partial class PlayerHandManager : HandManager
{
    [Export] PlayerDeckManager? playerDeckManager;
    private HBoxContainer? cardContainer;
    public List<BaseCardTemplate> playerCardsInHand = [];

    public override void _Ready()
    {
        cardContainer = GetNode<HBoxContainer>("CardContainer");
    }
    
    public override void GetTopCard()
    {
        if (playerDeckManager?.playerCardsInDeck.Count == 0) { /* i could make the game auto lose if this happens */ return;}
        if (playerCardsInHand.Count >= 10) { GD.Print("Hand is full"); return; }

        var cardInstance = playerDeckManager?.playerCardsInDeck[^1].Instantiate();

        if (cardInstance is not BaseCardTemplate card) return;
        
        card.GetOrAddComponent<DraggableComponent>();
            
        card.StateMachine.ChangeState(new CardInHand());
        cardContainer?.AddChild(card);
        playerCardsInHand.Add(card);
            
        playerDeckManager?.removeTopCardFromDeck();
    }

    public override void RemoveCardFromHand(BaseCardTemplate cardToRemove)
    {
        playerCardsInHand.Remove(cardToRemove);
        cardContainer?.RemoveChild(cardToRemove);
    }
    
    public override BaseCardTemplate? FindCard(int availableSouls)
    {
        GetTopCard(); // TEMP
        foreach (var card in playerCardsInHand)
        {
            if (card.CardData != null && card.CardData.Cost <= availableSouls)
            {
                return card;
            }
        }
        return null;
    }
}