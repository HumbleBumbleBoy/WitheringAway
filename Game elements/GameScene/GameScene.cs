using Godot;
using System;

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
    {
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
}
