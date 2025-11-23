using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Witheringaway.Game_elements.Cards.Units.BaseCardTemplate;
using Witheringaway.Game_elements.components;
using Witheringaway.Game_elements.lib;
using Witheringaway.Game_elements.lib.manager;

public partial class PlayerHandManager : HandManager
{
    [Export] PlayerDeckManager? playerDeckManager;
    private HBoxContainer? cardContainer;
    public List<BaseCardTemplate> playerCardsInHand = [];

    public override void _Ready()
    {
        cardContainer = GetNode<HBoxContainer>("CardContainer");
        PlayerHandManager = this;
    }
    
    public override void _ExitTree()
    {
        if (PlayerHandManager == this)
        {
            PlayerHandManager = null!;
        }
    }

    public override bool HasMoreCards()
    {
        return playerDeckManager?.PlayerCardsInDeck.Count > 0;
    }

    public override void GetTopCard()
    {
        if (playerDeckManager?.PlayerCardsInDeck.Count == 0) { /* i could make the game auto lose if this happens */ return;}
        if (playerCardsInHand.Count >= 10) { GD.Print("Hand is full"); return; }

        var cardInstance = playerDeckManager?.PlayerCardsInDeck[^1].Instantiate();

        if (cardInstance is not BaseCardTemplate card) return;
        
        card.GetOrAddComponent<DraggableComponent>();
            
        card.StateMachine.ChangeState(new CardInHand());
  
        playerCardsInHand.Add(card);
        playerCardsInHand.Sort((cardA, cardB) => 
        {
            if (cardA.CardData == null || cardB.CardData == null) return 0;
            return cardA.CardData.Cost.CompareTo(cardB.CardData.Cost); 
        });
        
        cardContainer?.AddChild(card);
        cardContainer?.MoveChild(card, Math.Min(playerCardsInHand.IndexOf(card), cardContainer.GetChildCount() - 1));
        
        playerDeckManager?.RemoveTopCardFromDeck();
    }

    public override void RemoveCardFromHand(BaseCardTemplate cardToRemove)
    {
        playerCardsInHand.Remove(cardToRemove);
        
        if (cardToRemove.GetParent() == cardContainer)
        {
            cardContainer?.RemoveChild(cardToRemove);   
        }
    }
    
    public override BaseCardTemplate? FindCard(int availableSouls, Predicate<BaseCardTemplate>? additionalCondition = null)
    {
        foreach (var card in playerCardsInHand.Where(card => additionalCondition == null || additionalCondition(card)))
        {
            if (card.CardData != null && card.CardData.Cost <= availableSouls)
            {
                return card;
            }
        }

        return null;
    }
}