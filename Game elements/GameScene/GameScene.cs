using Godot;
using System;
using System.Linq;

public partial class GameScene : Node2D
{
    private PlayerHandManager? playerHandManager;
    private PlayerDeckManager? playerDeckManager;

    public override void _Ready()
    {
        playerHandManager = GetNode<Node>("Player").GetNode<PlayerHandManager>("PlayerHandManager");
        playerDeckManager = GetNode<Node>("Player").GetNode<PlayerDeckManager>("PlayerDeckManager");
    }

    private void onDrawCardButtonPressed()
    {
        if (playerHandManager?.playerCardsInHand.Count < 10)
        {
            playerHandManager.GetTopCard();
        } else { GD.Print("Hand full"); }
    }

    private void onFillHandButton()
    {
        while (playerDeckManager?.playerCardsInDeck.Count > 0 && playerHandManager?.playerCardsInHand.Count < 10)
        {
            playerHandManager.GetTopCard();
        }
    }

    private void onRemoveCardButtonPressed()
    {   // this will not work always since it just might remove field cards sob
        playerHandManager?.RemoveCardFromHand(playerHandManager.playerCardsInHand[new Random().Next(0, playerHandManager.playerCardsInHand.Count)]);
    }

    private void onSpawnDeckButtonPressed()
    {
        int randomDeckIndex = new Random().Next(0, playerDeckManager?.Decks.Length??0);
        PackedScene[] selectedDeckCards = playerDeckManager?.Decks[randomDeckIndex].Cards??[];
        
        foreach (PackedScene packedScene in selectedDeckCards)
        {
            playerDeckManager?.addCardToDeck(packedScene);
            playerDeckManager?.addCardToDeckVisually(packedScene);
        }
    }

    private void onClearHandButtonPressed()
    {
        while (playerHandManager?.playerCardsInHand.Count > 0)
        {
            playerHandManager.RemoveCardFromHand(playerHandManager.playerCardsInHand[0]);
        }
    }

    private void onKillCardOnFieldButton()
    {
        FieldData fieldData = GetNode<FieldData>("/root/GameScene/FieldData");
    
        // Get all non-null cards on field
        var aliveCards = fieldData.playerCardsOnField.Where(card => card != null).ToList();
        
        if (aliveCards.Count > 0)
        {
            // Pick a random card and kill it
            var randomCard = aliveCards[new Random().Next(0, aliveCards.Count)];
            randomCard.ChangeState(new CardDied());
            GD.Print("Killed: " + randomCard.Name);
        }
        else
        {
            GD.Print("No cards on field to kill");
        }
    }
}
