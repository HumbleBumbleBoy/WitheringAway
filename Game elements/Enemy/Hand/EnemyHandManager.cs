using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Witheringaway.Game_elements.Cards;
using Witheringaway.Game_elements.Cards.Units.BaseCardTemplate;
using Witheringaway.Game_elements.components;
using Witheringaway.Game_elements.lib.manager;

public partial class EnemyHandManager : HandManager
{
    [Export] EnemyDeckManager? enemyDeckManager;
    private HBoxContainer? cardContainer;
    public List<BaseCardTemplate> enemyCardsInHand = [];

    public override void _Ready()
    {
        cardContainer = GetNode<HBoxContainer>("CardContainer");
        EnemyHandManager = this;
    }

    public override void _ExitTree()
    {
        if (EnemyHandManager == this)
        {
            EnemyHandManager = null!;
        }
    }

    public override bool HasMoreCards()
    {
        return enemyDeckManager?.enemyCardsInDeck.Count > 0;
    }

    public override void GetTopCard()
    {
        if (enemyDeckManager?.enemyCardsInDeck.Count == 0) { /* i could make the game auto lose if this happens */ return;}
        if (enemyCardsInHand.Count >= 10) { GD.Print("Hand is full"); return; }

        var cardInstance = enemyDeckManager?.enemyCardsInDeck[^1].Instantiate();

        if (cardInstance is not BaseCardTemplate card) return;
        GD.Print("Card added");
        card.RemoveAllComponents<DraggableComponent>();
        
        card.SetFlipped(true);
        
        enemyCardsInHand.Add(card);
        enemyCardsInHand.Sort((cardA, cardB) => 
        {
            if (cardA.CardData == null || cardB.CardData == null) return 0;
            return cardA.CardData.Cost.CompareTo(cardB.CardData.Cost); 
        });
        
        cardContainer?.AddChild(card);
        cardContainer?.MoveChild(card, Math.Min(enemyCardsInHand.IndexOf(card), cardContainer.GetChildCount() - 1));
            
        enemyDeckManager?.removeTopCardFromDeck();
    }

    public override void RemoveCardFromHand(BaseCardTemplate cardToRemove)
    {
        cardToRemove.SetFlipped(false);
        enemyCardsInHand.Remove(cardToRemove);
        cardContainer?.RemoveChild(cardToRemove);
    }

    public override BaseCardTemplate? FindCard(int availableSouls, Predicate<BaseCardTemplate>? additionalCondition = null)
    {
        foreach (var card in enemyCardsInHand.Where(card => additionalCondition == null || additionalCondition(card)))
        {
            if (card is TrickCard)
            {
                continue;
            }
            
            if (card.CardData != null && card.CardData.Cost <= availableSouls)
            {
                return card;
            }
        }
        
        return null;
    }
    
    
}