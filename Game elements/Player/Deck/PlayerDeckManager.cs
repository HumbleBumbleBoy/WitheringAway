using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Witheringaway.Game_elements.components;

public partial class PlayerDeckManager : Node
{
    [Export] public Deck[] Decks = [];
    private PlayerHandManager? handManager;
    public List<PackedScene> playerCardsInDeck = [];
    private VBoxContainer? cardsInDeckContainer;

    public override void _Ready()
    {
        cardsInDeckContainer = GetParent().GetNode<Control>("PlayerDeck").GetNode<VBoxContainer>("CardContainer");
        handManager = GetParent().GetNode<PlayerHandManager>("PlayerHandManager");

        var selectedDeck = Decks[new Random().Next(0, Decks.Length)];  // for now chosen randomly
        playerCardsInDeck.AddRange(selectedDeck.Cards);
        ShuffleDeck();

        foreach (PackedScene card in playerCardsInDeck)
        {
            addCardToDeckVisually(card);
        }
        GD.Print("Deck contains " + playerCardsInDeck.Count + " cards");
    }

    public void ShuffleDeck()
    {
        Random random = new();
        int n = playerCardsInDeck.Count;
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            (playerCardsInDeck[k], playerCardsInDeck[n]) = (playerCardsInDeck[n], playerCardsInDeck[k]);
        }
    }

    public void addCardToDeckVisually(PackedScene cardScene)
    {
        Node cardInstance = cardScene.Instantiate();
        if (cardInstance is BaseCardTemplate cardInDeck)
        {
            cardInDeck.StateMachine.ChangeState(new CardInDeck());
        }
        cardInstance.RemoveAllComponents<DraggableComponent>();
        cardsInDeckContainer?.AddChild(cardInstance);
    }

    public void addCardToDeck(PackedScene cardScene)
    {
        playerCardsInDeck.Add(cardScene);
        ShuffleDeck();
    }

    public void removeTopCardFromDeck()
    {
        cardsInDeckContainer?.RemoveChild(cardsInDeckContainer.GetChildren()[^1]);
        playerCardsInDeck.RemoveAt(playerCardsInDeck.Count-1);
    }
}