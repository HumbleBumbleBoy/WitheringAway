using Godot;
using System;

public partial class PlayerHandManager : Control
{
    [Export] PlayerDeckManager playerDeckManager;
    private HBoxContainer cardContainer;

    public override void _Ready()
    {
        cardContainer = GetNode<HBoxContainer>("CardContainer");
    }
    
    public void GetTopCard()
    {
        Node cardInstance = playerDeckManager.playerCardsInDeck[^1].Instantiate();
        cardContainer.AddChild(cardInstance);
        playerDeckManager.removeTopCardFromDeck();
    }
    
    public void RemoveCardFromHand(Control cardContainerToRemove)
    {
        cardContainer.RemoveChild(cardContainerToRemove);
        cardContainerToRemove.QueueFree();
    }
}