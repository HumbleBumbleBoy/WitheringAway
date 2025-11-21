using Godot;
using System;
using System.Collections.Generic;

public partial class EnemyDeckManager : Node
{
    [Export] public Deck[] Decks = [];
    private EnemyHandManager? handManager;
    public List<PackedScene> enemyCardsInDeck = [];
    private VBoxContainer? cardsInDeckContainer;

    public override void _Ready()
    {
        cardsInDeckContainer = GetParent().GetNode<Control>("EnemyDeck").GetNode<VBoxContainer>("CardContainer");
        handManager = GetParent().GetNode<EnemyHandManager>("EnemyHandManager");

        var selectedDeck = Decks[new Random().Next(0, Decks.Length)];  // for now chosen randomly
        enemyCardsInDeck.AddRange(selectedDeck.Cards);
        ShuffleDeck();

        foreach (PackedScene card in enemyCardsInDeck)
        {
            addCardToDeckVisually(card);
        }
        GD.Print("Deck contains " + enemyCardsInDeck.Count + " cards");
    }

    public void ShuffleDeck()
    {
        Random random = new();
        int n = enemyCardsInDeck.Count;
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            (enemyCardsInDeck[k], enemyCardsInDeck[n]) = (enemyCardsInDeck[n], enemyCardsInDeck[k]);
        }
    }

    public void addCardToDeckVisually(PackedScene cardScene)
    {
        Node cardInstance = cardScene.Instantiate();
        if (cardInstance is BaseCardTemplate cardInDeck)
        {
            cardInDeck.ChangeState(new CardInDeck());
        }
        cardsInDeckContainer?.AddChild(cardInstance);
    }

    public void addCardToDeck(PackedScene cardScene)
    {
        enemyCardsInDeck.Add(cardScene);
        ShuffleDeck();
    }

    public void removeTopCardFromDeck()
    {
        cardsInDeckContainer?.RemoveChild(cardsInDeckContainer.GetChildren()[^1]);
        enemyCardsInDeck.RemoveAt(enemyCardsInDeck.Count-1);
    }
}