using Godot;
using Witheringaway.Game_elements.lib;

public class Combat : IState<TurnManager>
{
    
    private EnemyHandManager? enemyHandManager;
    private PlayerHandManager? playerHandManager;
    private FieldData? fieldData;
    
    public IState<TurnManager>? OnEnter(TurnManager turnManager, IState<TurnManager>? previousState)
    {
        turnManager.isCombatTime = true;
        
        enemyHandManager = turnManager.GetTree().GetFirstNodeInGroup("EnemyHandManager") as EnemyHandManager;
        playerHandManager = turnManager.GetTree().GetFirstNodeInGroup("PlayerHandManager") as PlayerHandManager;
        fieldData = turnManager.GetNode<FieldData>("/root/GameScene/FieldData");;
        
        return null;
    }

    public IState<TurnManager>? OnExit(TurnManager turnManager, IState<TurnManager>? nextState)
    {
        turnManager.isCombatTime = false;
        return null;
    }

    private void BeginCombat()
    {
        for (int i = 0; i < 5; i++)
        {
            Fight(i);            
        }
    }

    private void Fight(int lane)
    {
        var playerCard = fieldData?.GetCardOnSpecificLane(lane, true);
        var enemyCard = fieldData?.GetCardOnSpecificLane(lane, false);

        if (playerCard == null || enemyCard == null)
        {
            
        }
    }
}