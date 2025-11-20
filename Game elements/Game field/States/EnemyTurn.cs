using Godot;
using System;
using System.Threading;
using System.Threading.Tasks;

public partial class EnemyTurn : TurnState
{
    private EnemyHandManager? enemyHandManager;
    private Node? enemyDeckManager;
    private HBoxContainer? enemyCardContainer;

    public override void Enter(TurnManager turnManager)
    {
        turnManager.canEnemyPlaceCards = true; // is even needed??

        enemyDeckManager = turnManager.GetParent().GetNode<Node>("Enemy").GetNode<Node>("EnemyDeckManager");
        enemyHandManager = turnManager.GetParent().GetNode<Node>("Enemy").GetNode<EnemyHandManager>("EnemyHandManager");
        enemyCardContainer = enemyHandManager.GetNode<HBoxContainer>("CardContainer");
    }

    public override void Exit(TurnManager turnManager)
    {
        turnManager.canEnemyPlaceCards = false;
    }

    private async Task PlayCards(CancellationToken cancellationToken)
    {
        
    }

    private async Task PlaceSingleCard(CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            cancellationToken.ThrowIfCancellationRequested(); 
            return;
        }   
    }
}
