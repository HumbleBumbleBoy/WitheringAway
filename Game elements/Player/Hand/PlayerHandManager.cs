using Godot;
using System;
using System.Collections.Generic;

public partial class PlayerHandManager : Control
{
    [Export] PlayerDeckManager playerDeckManager;
    private HBoxContainer cardContainer;
    public List<Node> playerCardsInHand = [];

    public override void _Ready()
    {
        cardContainer = GetNode<HBoxContainer>("CardContainer");
    }
    
    public void GetTopCard()
    {
        Node cardInstance = playerDeckManager.playerCardsInDeck[^1].Instantiate();
        cardContainer.AddChild(cardInstance);
        playerCardsInHand.Add(cardInstance);
        playerDeckManager.removeTopCardFromDeck();
    }
    
    public void RemoveCardFromHand(Node cardToRemove)
    {
        playerCardsInHand.Remove(cardToRemove);
        cardContainer.RemoveChild(cardToRemove);
        cardToRemove.QueueFree();
    }
}