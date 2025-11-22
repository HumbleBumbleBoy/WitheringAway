using System.Threading.Tasks;
using Godot;
using Witheringaway.Game_elements.lib;

public class Combat : IState<TurnManager>
{
    private FieldData? fieldData;
    
    public IState<TurnManager>? OnEnter(TurnManager turnManager, IState<TurnManager>? previousState)
    {
        GD.Print("FIGHT!");
        turnManager.isCombatTime = true;
        
        fieldData = turnManager.GetNode<FieldData>("/root/GameScene/FieldData");
        
        BeginCombat(turnManager);
        
        return null;
    }

    public IState<TurnManager>? OnExit(TurnManager turnManager, IState<TurnManager>? nextState)
    {
        turnManager.isCombatTime = false;
        return null;
    }

    private async void BeginCombat(TurnManager turnManager)
    {
        for (var i = 0; i < 5; i++)
        {
            await turnManager.Wait(0.5f);
            await Fight(turnManager, i);            
        }
        
        turnManager.StateMachine.ChangeState(new Transition());
    }

    private async Task Fight(TurnManager turnManager, int lane)
    {
        var playerCard = fieldData?.GetCardOnSpecificLane(lane, true);
        var enemyCard = fieldData?.GetCardOnSpecificLane(lane, false);

        if (playerCard == null && enemyCard == null)
        {
            return; // nothing to fight
        }

        if (playerCard == null)
        {
            return; // damage player
        }

        if (enemyCard == null)
        {
            return; // damage enemy
        }

        await playerCard.Attack(enemyCard);
        await enemyCard.Attack(playerCard);

        await turnManager.Wait(0.5f);

        if (playerCard.ShouldDie())
        {
            playerCard.Wait(0.2f).ContinueWith(_ => playerCard.CallDeferred(nameof(BaseCardTemplate.DisableArt)));

            await Task.WhenAll(
                playerCard.PlayAnimation("Dying", 0.2f),
                playerCard.PlaySound("Death")
            );
            playerCard.QueueFree();
        }
        
        if (enemyCard.ShouldDie())
        {
            enemyCard.Wait(0.2f).ContinueWith(_ => enemyCard.CallDeferred(nameof(BaseCardTemplate.DisableArt)));
            
            await Task.WhenAll(
                enemyCard.PlayAnimation("Dying", 0.2f),
                enemyCard.PlaySound("Death")
            );
            enemyCard.QueueFree();
        }
    }

}