using System.Threading.Tasks;
using Godot;
using Witheringaway.Game_elements.lib;
using Witheringaway.Game_elements.lib.manager;

public class PlayerTurn : IState<TurnManager>
{
    public IState<TurnManager>? OnEnter(TurnManager context, IState<TurnManager>? previousState)
    {
        context.canPlayerPlaceCards = true;
        context.EnablePassTurnButton();

        SetupFirstRound(context);
        
        return null;
    }

    public IState<TurnManager>? OnExit(TurnManager context, IState<TurnManager>? nextState)
    {
        context.DisalePassTurnButton();
        context.canPlayerPlaceCards = false;
        return null;
    }
    
    private async Task SetupFirstRound(TurnManager context)
    {
        await context.Wait(0.1f);

        if (HandManager.PlayerHandManager.GetNodeOrNull("first_round_setup_done") != null)
        {
            context.GetNode<AudioStreamPlayer>("/root/GameScene/TurnChange").Play();
            return;
        }
        
        // first round setup
        var firstRoundSetupDone = new Node();
        firstRoundSetupDone.Name = "first_round_setup_done";
        HandManager.PlayerHandManager.AddChild(firstRoundSetupDone);
        
        for (var i = 0; i < 5; i++)
        {
            await context.Wait(0.1f);
            HandManager.PlayerHandManager?.GetTopCard();
        }
    }

}
