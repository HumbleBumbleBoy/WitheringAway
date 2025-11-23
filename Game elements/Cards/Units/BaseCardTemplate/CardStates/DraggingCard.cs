using Godot;
using Witheringaway.Game_elements.Cards.Units.BaseCardTemplate;
using Witheringaway.Game_elements.lib;

public class DraggingCard : IState<BaseCardTemplate>
{
    private Vector2 _originalPosition;

    public IState<BaseCardTemplate>? OnEnter(BaseCardTemplate card, IState<BaseCardTemplate>? previousState)
    {
        card.AudioFolder?.GetNode<AudioStreamPlayer>("PickUp").Play();
        _originalPosition = card.GlobalPosition; // Save the original position

        return null;
    }

    public IState<BaseCardTemplate>? OnExit(BaseCardTemplate card, IState<BaseCardTemplate>? nextState)
    {
        if (nextState is CardInHand)
        {
            card.GlobalPosition = _originalPosition;
        }

        return null;
    }

    public IState<BaseCardTemplate>? OnUpdate(BaseCardTemplate card, double deltaTime)
    {
        if (!card.ShouldKeepDragging())
        {
            return null;
        }
        
        card.GlobalPosition = card.GetGlobalMousePosition() - card.GetGlobalRect().Size / 2;
        
        return null;
    }
}