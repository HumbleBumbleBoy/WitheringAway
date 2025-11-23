using System;
using System.Collections.Generic;
using Godot;
using Witheringaway.Game_elements.Cards.Units.BaseCardTemplate;
using Witheringaway.Game_elements.components;

public partial class PlayerDeckManager : Node
{
    [Export] public Deck[] Decks = [];
    public List<PackedScene> PlayerCardsInDeck = [];
    
    private VBoxContainer? cardsInDeckContainer;

    public override void _Ready()
    {
        cardsInDeckContainer = GetParent().GetNode<Control>("PlayerDeck").GetNode<VBoxContainer>("CardContainer");

        var selectedDeck = Decks[new Random().Next(0, Decks.Length)];
        PlayerCardsInDeck.AddRange(selectedDeck.Cards);
        ShuffleDeck();

        foreach (var card in PlayerCardsInDeck)
        {
            AddCardToDeckVisually(card);
        }
    }
    
    public void RemoveTopCardFromDeck()
    {
        cardsInDeckContainer?.RemoveChild(cardsInDeckContainer.GetChildren()[^1]);
        PlayerCardsInDeck.RemoveAt(PlayerCardsInDeck.Count-1);
    }

    private void ShuffleDeck()
    {
        Random random = new();
        var n = PlayerCardsInDeck.Count;
        while (n > 1)
        {
            n--;
            var k = random.Next(n + 1);
            (PlayerCardsInDeck[k], PlayerCardsInDeck[n]) = (PlayerCardsInDeck[n], PlayerCardsInDeck[k]);
        }
    }

    private void AddCardToDeckVisually(PackedScene cardScene)
    {
        var cardInstance = cardScene.Instantiate();
        if (cardInstance is BaseCardTemplate cardInDeck)
        {
            cardInDeck.StateMachine.ChangeState(new CardInDeck());
        }
        cardInstance.RemoveAllComponents<DraggableComponent>();
        cardsInDeckContainer?.AddChild(cardInstance);
    }
}