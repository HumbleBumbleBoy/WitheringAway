using Godot;
using Witheringaway.Game_elements.lib;

public class DraggingCard : IState<BaseCardTemplate>
{
    private Vector2 _originalPosition;

    public IState<BaseCardTemplate>? OnEnter(BaseCardTemplate card, IState<BaseCardTemplate>? previousState)
    {
        GD.Print("dragging " + card.Name);
        card.audioFolder?.GetNode<AudioStreamPlayer>("PickUp").Play();
        _originalPosition = card.GlobalPosition; // Save the original position

        return null;
    }

    public IState<BaseCardTemplate>? OnExit(BaseCardTemplate card, IState<BaseCardTemplate>? nextState)
    {
        GD.Print("stopped dragging " + card.Name);

        if (nextState is CardInHand)
        {
            card.GlobalPosition = _originalPosition;
        }

        return null;
    }

    public IState<BaseCardTemplate>? OnUpdate(BaseCardTemplate card, double deltaTime)
    {
        GD.Print("dragging update " + card.Name);
        
        card.GlobalPosition = card.GetGlobalMousePosition() - card.GetGlobalRect().Size / 2;
        
        return null;
    }
}