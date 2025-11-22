using System.Threading.Tasks;
using Godot;
using Witheringaway.Game_elements.lib;

public class Combat : IState<TurnManager>
{
    private FieldData? fieldData;
    
    public IState<TurnManager>? OnEnter(TurnManager turnManager, IState<TurnManager>? previousState)
    {
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
        for (int i = 0; i < 5; i++)
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
            GD.Print("Damaging player!");
            return; // damage player
        }

        if (enemyCard == null)
        {
            GD.Print("Damaging enemy!");
            return; // damage enemy
        }

        await playerCard.Attack(enemyCard);
        await enemyCard.Attack(playerCard);

        await turnManager.Wait(0.5f);

        if (playerCard.ShouldDie())
        {
            playerCard.Wait(0.2f).ContinueWith(task => playerCard.CallDeferred("hide"));
            await playerCard.PlaySound("Death");
            playerCard.QueueFree();
        }
        
        if (enemyCard.ShouldDie())
        {
            enemyCard.Wait(0.2f).ContinueWith(task => enemyCard.CallDeferred("hide"));
            await enemyCard.PlaySound("Death");
            enemyCard.QueueFree();
        }
    }

}