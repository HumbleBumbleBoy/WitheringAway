using Godot;
using System;
using System.Collections.Generic;

public partial class EnemyDeckManager : Node
{
    [Export] public Deck[] Decks = [];
    private EnemyHandManager? handManager;
    public List<PackedScene> enemyCardsInDeck = [];

    public override void _Ready()
    {
        handManager = GetParent().GetNode<EnemyHandManager>("EnemyHandManager");

        var selectedDeck = Decks[new Random().Next(0, Decks.Length)];  // for now chosen randomly
        enemyCardsInDeck.AddRange(selectedDeck.Cards);
        ShuffleDeck();
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

    public void addCardToDeck(PackedScene cardScene)
    {
        enemyCardsInDeck.Add(cardScene);
        ShuffleDeck();
    }

    public void removeTopCardFromDeck()
    {
        enemyCardsInDeck.RemoveAt(enemyCardsInDeck.Count-1);
    }
}