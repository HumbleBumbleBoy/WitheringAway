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
        foreach (PackedScene packedScene in playerDeckManager?.Decks[0].Cards??[])
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
