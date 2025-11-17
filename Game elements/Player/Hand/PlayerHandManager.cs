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
        Node? cardInstance = playerDeckManager?.playerCardsInDeck[^1].Instantiate();
        
        // Cast to BaseCardTemplate to access ChangeState
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
        // since this ALWAYS means the player had to manually place the card we change the state to placed, since this will be called after dragging ends
        if (cardToRemove is BaseCardTemplate card)
        {
            card.ChangeState(new CardEnteredField());
            playerCardsInHand.Remove(card);
            cardContainer?.RemoveChild(card);
            card.QueueFree();
        }
    }
}