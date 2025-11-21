using Godot;
using System;
using System.Collections.Generic;

public partial class EnemyHandManager : Control
{
    [Export] EnemyDeckManager? enemyDeckManager;
    private HBoxContainer? cardContainer;
    public List<Node> enemyCardsInHand = [];

    public override void _Ready()
    {
        cardContainer = GetNode<HBoxContainer>("CardContainer");
    }
    
    public void GetTopCard()
    {
        if (enemyDeckManager?.enemyCardsInDeck.Count == 0) { /* i could make the game auto lose if this happens */ return;}
        if (enemyCardsInHand.Count >= 10) { GD.Print("Hand is full"); return; }

        Node? cardInstance = enemyDeckManager?.enemyCardsInDeck[^1].Instantiate();
        
        if (cardInstance is BaseCardTemplate card)
        {
            card.ChangeState(new CardInHand());
            cardContainer?.AddChild(card);
            enemyCardsInHand.Add(card);
            
            enemyDeckManager?.removeTopCardFromDeck();
        }
    }

    public void RemoveCardFromHand(Node cardToRemove)  
    {
        if (cardToRemove is BaseCardTemplate card)
        {
            enemyCardsInHand.Remove(card);
            cardContainer?.RemoveChild(card);
        }
    }
}