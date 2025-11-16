using Godot;
using System;

public partial class GameScene : Node2D
{
    private PlayerHandManager playerHandManager;
    private PlayerDeckManager playerDeckManager;

    public override void _Ready()
    {
        playerHandManager = GetNode<Node>("Player").GetNode<PlayerHandManager>("PlayerHandManager");
        playerDeckManager = GetNode<Node>("Player").GetNode<PlayerDeckManager>("PlayerDeckManager");

        playerHandManager.GetTopCard();
        GD.Print(playerDeckManager.playerCardsInDeck.Count);
    }
}
