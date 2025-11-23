using System.Diagnostics;
using System.Threading.Tasks;
using Godot;
using Witheringaway.Game_elements.Cards.Units.BaseCardTemplate;
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
            if (turnManager.StateMachine.CurrentState != this)
            {
                return;
            }
            
            await turnManager.Wait(0.5f);
            await Fight(turnManager, i);            
        }
        
        turnManager.StateMachine.ChangeState(new Transition());
    }

    private async Task Fight(TurnManager turnManager, int lane)
    {
        if (turnManager.StateMachine.CurrentState != this)
        {
            return;
        }
        
        var playerCard = fieldData?.GetCardOnSpecificLane(lane, true);
        var enemyCard = fieldData?.GetCardOnSpecificLane(lane, false);

        if (playerCard == null && enemyCard == null)
        {
            return; // nothing to fight
        }

        if (playerCard == null)
        {
            Debug.Assert(enemyCard != null, nameof(enemyCard) + " != null");
            
            var playerDuelist = Duelist.PlayerDuelist;
            await enemyCard.Attack(playerDuelist, Callable.From(() =>
            {
                _ = enemyCard.PlaySound("Attack");
                _ = playerDuelist.TakeDamage(enemyCard.GetAttackDamage());
                _ = enemyCard.PlaySound("Hurt");
            }));
            return;
        }

        if (enemyCard == null)
        {
            Debug.Assert(playerCard != null, nameof(playerCard) + " != null");
            
            var enemyDuelist = Duelist.EnemyDuelist;
            await playerCard.Attack(enemyDuelist, Callable.From(() =>
            {
                _ = playerCard.PlaySound("Attack");
                _ = enemyDuelist.TakeDamage(playerCard.GetAttackDamage());
                _ = playerCard.PlaySound("Hurt");
            }));
            return;
        }

        await playerCard.Attack(enemyCard);
        await enemyCard.Attack(playerCard);

        await turnManager.Wait(0.5f);

        if (playerCard.ShouldDie())
        {
            playerCard.Wait(0.2f).ContinueWith(_ => playerCard.CallDeferred(nameof(BaseCardTemplate.DisableArt)));

            await playerCard.Kill();
        }
        
        if (enemyCard.ShouldDie())
        {
            enemyCard.Wait(0.2f).ContinueWith(_ => enemyCard.CallDeferred(nameof(BaseCardTemplate.DisableArt)));
            
            await enemyCard.Kill();
        }
    }

}