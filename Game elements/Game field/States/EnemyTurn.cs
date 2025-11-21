using Godot;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public partial class EnemyTurn : TurnState
{
    private EnemyHandManager? enemyHandManager;
    private Node? enemyDeckManager;
    private HBoxContainer? enemyCardContainer;
    private TurnManager? _turnManager;
    private CancellationTokenSource? tokenSource = new();

    public override async void Enter(TurnManager turnManager)
    {
        _turnManager = turnManager;
        turnManager.canEnemyPlaceCards = true; // is even needed??

        enemyDeckManager = turnManager.GetParent().GetNode<Node>("Enemy").GetNode<Node>("EnemyDeckManager");
        enemyHandManager = turnManager.GetParent().GetNode<Node>("Enemy").GetNode<EnemyHandManager>("EnemyHandManager");
        enemyCardContainer = enemyHandManager.GetNode<HBoxContainer>("CardContainer");

        try
        {
            await PlayCards(tokenSource.Token);
        } catch (OperationCanceledException _){} 
    }

    public override void Exit(TurnManager turnManager)
    {
        turnManager.canEnemyPlaceCards = false;
        tokenSource.Dispose();
    }

    private async Task PlayCards(CancellationToken cancellationToken)
    {
        var enemyPlacablePositions = _turnManager.GetTree().GetNodesInGroup("EnemyPlacablePosition");
        foreach (var position in enemyPlacablePositions)
        {
            var positionTaken = position.GetChildren().Where(child => child is BaseCardTemplate).Any();

            if (!positionTaken)
            {
                await position.ToSignal(position.GetTree().CreateTimer(.1), "timeout");
                await PlaceSingleCard(cancellationToken, position);
            }
        }
    }

    private async Task PlaceSingleCard(CancellationToken cancellationToken, Node cardPosition)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            cancellationToken.ThrowIfCancellationRequested(); 
            return;
        }   

        var card = enemyHandManager.PlayCard(100); // so rich
        
        if (card == null) 
        { 
            tokenSource.Cancel();
            _turnManager.ChangeState(new Combat());
            return;
        }

        enemyHandManager.RemoveCardFromHand(card);
        cardPosition.AddChild(card);
    }
}
