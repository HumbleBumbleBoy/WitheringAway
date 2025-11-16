using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class PlayerDeckManager : Node
{
    [Export] private Deck[] Decks = [];
    private PlayerHandManager handManager;
    public List<PackedScene> playerCardsInDeck = [];
    private VBoxContainer cardsInDeckContainer;

    public override void _Ready()
    {
        cardsInDeckContainer = GetParent().GetNode<Control>("PlayerDeck").GetNode<VBoxContainer>("CardContainer");
        handManager = GetParent().GetNode<PlayerHandManager>("PlayerHandManager");
        playerCardsInDeck.AddRange(Decks[new Random().Next(0, Decks.Length)].Cards);  // for now the deck chosen is random

        foreach (PackedScene card in playerCardsInDeck)
        {
            addCardToDeckVisually(card);
        }
        GD.Print("Deck contains " + playerCardsInDeck.Count + " cards");
    }

    public void addCardToDeckVisually(PackedScene cardScene)
    {
        Node cardInstance = cardScene.Instantiate();
        if (cardInstance is BaseCardTemplate cardInDeck)
        {
            cardInDeck.isFlipped = true;
        }
        cardsInDeckContainer.AddChild(cardInstance);
    }

        public void addCardToDeck(PackedScene cardScene)
    {
        playerCardsInDeck.Add(cardScene);
    }

    public void removeTopCardFromDeck()
    {
        cardsInDeckContainer.RemoveChild(cardsInDeckContainer.GetChildren()[^1]);
        playerCardsInDeck.RemoveAt(playerCardsInDeck.Count-1);
    }
}