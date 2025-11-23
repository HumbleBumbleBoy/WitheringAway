using System.Collections.Generic;
using Godot;
using Witheringaway.Game_elements.Cards.BaseCardTemplate;
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
        
        card.IsFlipped = true;
            
        cardContainer?.AddChild(card);
        enemyCardsInHand.Add(card);
            
        enemyDeckManager?.removeTopCardFromDeck();
    }

    public override void RemoveCardFromHand(BaseCardTemplate cardToRemove)
    {
        cardToRemove.IsFlipped = false;
        enemyCardsInHand.Remove(cardToRemove);
        cardContainer?.RemoveChild(cardToRemove);
    }

    public override BaseCardTemplate? FindCard(int availableSouls)
    {
        foreach (var card in enemyCardsInHand)
        {
            if (card.CardData != null && card.CardData.Cost <= availableSouls)
            {
                return card;
            }
        }
        return null;
    }
}