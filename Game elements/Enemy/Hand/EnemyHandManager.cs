using Godot;
using System;

public partial class EnemyHandManager : Control
{
    private HBoxContainer? cardContainer;

    public override void _Ready()
    {
        cardContainer = GetNode<HBoxContainer>("CardContainer");
    }
    
    public Control? AddCardToHand(PackedScene cardScene)
    {
        Node cardInstance = cardScene.Instantiate();
        if (cardInstance is BaseCardTemplate enemyCard)
        {
            enemyCard.isFlipped = true;
        }
        cardContainer?.AddChild(cardInstance);
        return cardContainer;
    }
    
    public void RemoveCardFromHand(Control cardContainerToRemove)
    {
        cardContainer?.RemoveChild(cardContainerToRemove);
        cardContainerToRemove.QueueFree();
    }
}