using Godot;
using System;

public partial class DraggingCard : CardState
{
    private Vector2 _originalPosition;
    private BaseCardTemplate? _card;
    
    public override void Enter(BaseCardTemplate card, ref CardState? optionalState)
    {
        GD.Print("dragging " + card.Name);
        card.audioFolder?.GetNode<AudioStreamPlayer>("PickUp").Play();
        _card = card;
        _originalPosition = _card.GlobalPosition; // Save the original position
    }

    public override void Exit(BaseCardTemplate card, ref CardState? optionalState)
    {
        GD.Print("stopped dragging " + card.Name);
        
        // If we're transitioning to CardInHand (invalid drop), return to original position
        if (optionalState is CardInHand)
        {
            _card.GlobalPosition = _originalPosition;
        }
        // If we're transitioning to CardEnteredField (valid drop), keep the current position
    }

    public override void Update(double delta)
    {
        if (_card != null)
        {
            // THIS IS WHAT MAKES THE CARD FOLLOW THE MOUSE
            _card.GlobalPosition = _card.GetGlobalMousePosition() - _card.GetGlobalRect().Size / 2;
        }
    }
}