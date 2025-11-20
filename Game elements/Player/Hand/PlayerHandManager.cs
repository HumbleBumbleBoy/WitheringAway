using Godot;
using System;
using System.Collections.Generic;

public partial class PlayerHandManager : Control
{
    [Export] PlayerDeckManager? playerDeckManager;
    private HBoxContainer? cardContainer;
    public List<Node> playerCardsInHand = [];

    public override void _Ready()
    {
        cardContainer = GetNode<HBoxContainer>("CardContainer");
    }
    
    public void GetTopCard()
    {
        if (playerDeckManager?.playerCardsInDeck.Count == 0)
        {
            // i could make the game auto lose if this happens
            return;
        }

        Node? cardInstance = playerDeckManager?.playerCardsInDeck[^1].Instantiate();
        
        if (cardInstance is BaseCardTemplate card)
        {
            card.ChangeState(new CardInHand());
            cardContainer?.AddChild(card);
            playerCardsInHand.Add(card);
            
            playerDeckManager?.removeTopCardFromDeck();
        }
    }

    public void RemoveCardFromHand(Node cardToRemove)  
    {
        if (cardToRemove is BaseCardTemplate card)
        {
            playerCardsInHand.Remove(card);
            cardContainer?.RemoveChild(card);
        }
    }
}